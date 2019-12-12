# Chat .NetCore WebApi

## Loopback bypass
-> netsh interface portproxy add v4tov4 listenaddress=xxx.xxx.xxx.xxx listenport = 5000 connectaddress=127.0.0.1 connectport=5000 

-> netsh interface portproxy show all

-> netsh interface portproxy delete v4tov4 listenaddress=xxx.xxx.xxx.xxx listenport = 5000


• listenaddress - локальный адрес на котором принимаются соединения

• listenport - локальный порт на котором принимаются соединения

• connectaddress - удаленный или локальный адрес на который перенаправляются соединения

• connectport - удаленный или локальный порт на который перенаправляются соединения
