FROM mcr.microsoft.com/mssql/server

USER root

ENV SA_PASSWORD This1sMyPassword
ENV ACCEPT_EULA Y

RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

COPY . /usr/src/app/

RUN chmod +x /usr/src/app/run-initialisation.sh

USER mssql
CMD /bin/bash ./entrypoint.sh
