@echo off

echo enter "exit" to stop
echo enter c, l or ch to start a specific service (central, login and channel respectively)
echo alternatively, enter "all" to start all 3 services

:start
set /P OPT=%=%

if /I "%OPT%"=="c" goto start_central
if /I "%OPT%"=="l" goto start_login
if /I "%OPT%"=="ch" goto start_channel
if /I "%OPT%"=="all" goto start_central
if /I "%OPT%"=="exit" goto exit

echo invalid command "%OPT%"... try again
ping 127.0.0.1 -n 3 > nul
goto start

:start_central
echo Starting Central server
start /d "Common\bin\Release\netcoreapp3.1" cmd /k "title Central Server & Common.exe"
ping 127.0.0.1 -n 2 > nul
if /I "%OPT%"=="c" goto start

:start_login
echo Starting Login server
start /d "Login\bin\Release\netcoreapp3.1" cmd /k "title Login Server & Login.exe"
ping 127.0.0.1 -n 2 > nul
if /I "%OPT%"=="l" goto start

:start_channel
echo Starting Channel server
start /d "Channels\bin\Release\netcoreapp3.1" cmd /k "title Channel Server & Channels.exe --world=0 --channels=0-2"
ping 127.0.0.1 -n 2 > nul
if /I "%OPT%"=="ch" goto start

:exit
if /I "%OPT%"=="all" goto start
pause