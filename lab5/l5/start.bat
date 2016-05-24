@echo off
set /a hive_capacity=8
set /a bees_count=5

start Lab_4\bin\Debug\Lab_4.exe barbershop %hive_capacity%
for /l %%A in (1, 1, %bees_count%) do (
	start Lab_4\bin\Debug\Lab_4.exe client
)
start Lab_4\bin\Debug\Lab_4.exe barber

pause

taskkill /im Lab_4.exe /f