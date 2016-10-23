
# Nano Server 2016

# Set up Nano Server

* ## **Prepare:** Hyper-V on your local machine

    1. [Enable Hyper-V](https://msdn.microsoft.com/en-us/virtualization/hyperv_on_windows/quick_start/walkthrough_install)

        With PowerShell

        ```PowerShell
        Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V -All
        ```
        ...or with DISM

        ```CMD
        DISM /Online /Enable-Feature /All /FeatureName:Microsoft-Hyper-V
        ```
    2. [Create a Virtual Switch](https://msdn.microsoft.com/en-us/virtualization/hyperv_on_windows/quick_start/walkthrough_virtual_switch)

       ```PowerShell
       # Use Get-NetAdapter to return a list of network adapters connected to the Windows 10 system.
       
       PS C:\> Get-NetAdapter

       Name                      InterfaceDescription                    ifIndex Status       MacAddress             LinkSpeed
       ----                      --------------------                    ------- ------       ----------             ---------
       Ethernet 2                Broadcom NetXtreme 57xx Gigabit Cont...       5 Up           BC-30-5B-A8-C1-7F         1 Gbps
       Ethernet                  Intel(R) PRO/100 M Desktop Adapter            3 Up           00-0E-0C-A8-DC-31        10 Mbps
      
       # Select the network adapter to use with the Hyper-V switch and place an instance in a variable named $net.

       PS C:\> $net = Get-NetAdapter -Name 'Ethernet'

       # Execute the following command to create the new Hyper-V virtual switch.

       PS C:\> New-VMSwitch -Name "External VM Switch" -AllowManagementOS $True -NetAdapterName $net.Name

       ```

    3. **Download:** [Windows Assessment and Deployment Kit (ADK)](https://developer.microsoft.com/en-us/windows/hardware/windows-assessment-deployment-kit)
       
       **Install:** `Windows Preinstallation Environment (Windows PE)`
    
    4. **Download:** [Windows Server 2016](https://www.microsoft.com/en-us/evalcenter/evaluate-windows-server-2016)
    
       **Extract** the Windows Server 2016 ISO to your hard drive or mount as virtual drive
    
    5. **Download:** [Nano Server Image Builder](https://www.microsoft.com/en-us/download/details.aspx?id=54065)
    6. **Create:** The container host virtual machine
    
        * Start Nano Server Image Builder
        * Create new Nano Server image
        * **Virtual machine image:** `C:\Hyper-V\Images\Nano.Container.vhdx` <-- VHDX! Not VHD.
        * **Nano server edition:** `Datacenter`
        * **Optional packages:** `Containers`
        * **Select:** Enable WinRM and remote PowerShell connections from all subnets
        * Once created, save the PowerShell command for later Setup
     
        ```PowerShell
        New-NanoServerImage 
           -MediaPath 'C:\path\to\your\en_windows_server_2016_x64_dvd_9327751'
           -Edition 'Datacenter'
           -DeploymentType Guest
           -TargetPath 'C:\path\to\your\ContainerVM.vhdx'
           -MaxSize 8589934592
           -EnableRemoteManagementPort
           -SetupUI ('NanoServer.Containers')
           -ComputerName 'ContainerVM'
           -SetupCompleteCommand ('tzutil.exe /s "FLE Standard Time"')
        ```
    
    7. **Create:** New Virtual Machine with Hyper-VContainerVM
        * Make it Generation 2
        * Pick the Switch you created earlier
        * Pick the Virtual Hard Disk you created
        * Start the Virtual Machine
    
    8. Ensure WinRM is running on LOCAL machine
        
        ```CMD
        net start winrm
        ```
    
    9. Add the virtual machine to your trusted hosts
    
       ```PowerShell
       Set-Item WSMan:\localhost\Client\TrustedHosts 192.168.1.50 -Force
       ```
    
    10. Create the remote PowerShell session.
    
        ```PowerShell
        Enter-PSSession -ComputerName 192.168.100.19 -Credential ~\Administrator
        ```
    
    11. Install Windows Updates
    
        ```PowerShell
        $sess = New-CimInstance -Namespace root/Microsoft/Windows/WindowsUpdate -ClassName MSFT_WUOperationsSession
        Invoke-CimMethod -InputObject $sess -MethodName ApplyApplicableUpdates
        ```
    
    12. Reboot the system once the updates have been applied.

        ```PowerShell
        Restart-Computer
        ```

* ## **Set Up:** Docker and Container Images

    1. **Install:** OneGet PowerShell Module

       ```PowerShell
       Install-Module -Name DockerMsftProvider -Repository PSGallery -Force
       ```
    
    2. **Install:** Latest version of Docker.

       ```PowerShell
       Install-Package -Name docker -ProviderName DockerMsftProvider

       ```
    
    3. **Reboot:** Virtual Machine

       ```PowerShell
       Restart-Computer -Force
       ```

    4. **Install:** Base Container Images

       ```PowerShell
       docker pull microsoft/nanoserver
       ```

       ```PowerShell
       docker pull microsoft/windowsservercore
       ```

* ## **Set Up:** [Manage Docker on Nano Server](https://msdn.microsoft.com/en-us/virtualization/windowscontainers/deployment/deployment_nano#manage-docker-on-nano-server)

    1. **Prepare:** Container Host

       ```PowerShell
       netsh advfirewall firewall add rule name="Docker daemon " dir=in action=allow protocol=TCP localport=2375
       ```

    2. **Configure:** Docker Engine to accept incoming connection over TCP

       ```PowerShell
       New-Item -Type File c:\ProgramData\docker\config\daemon.json
       ```

       ```PowerShell
       Add-Content 'c:\programdata\docker\config\daemon.json' '{ "hosts": ["tcp://0.0.0.0:2375", "npipe://"] }'
       ```

    3. **Restart:** Docker service.

       ```PowerShell
       Restart-Service docker
       ```
       