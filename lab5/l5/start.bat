@echo off
set /a barbershop_capacity=8
set /a clients_count=5

start Lab_4\bin\Debug\Lab_4.exe barbershop %barbershop_capacity%
for /l %%A in (1, 1, %clients_count%) do (
	start Lab_4\bin\Debug\Lab_4.exe client
)
start Lab_4\bin\Debug\Lab_4.exe barber

pause

taskkill /im Lab_4.exe /f