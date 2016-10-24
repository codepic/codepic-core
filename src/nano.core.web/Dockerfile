FROM microsoft/nanoserver

RUN dism /online /enable-feature /all /featurename:IIS-WindowsAuthentication

RUN echo "Hello World - Dockerfile" > c:\inetpub\wwwroot\index.html

CMD [ "cmd" ]

#COPY . /app

#WORKDIR /app

#RUN ["dotnet", "restore"]

#RUN ["dotnet", "build"]

#EXPOSE 5000/tcp

#ENTRYPOINT ["dotnet", "nano.core.web.dll"]

#CMD ["xcopy", "c:\windows\system32\forwarders"]

#CMD ["dotnet", "run", "--server.urls", "http://*:5000"]
