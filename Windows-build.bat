REM Building
call git submodule update --init
call %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe MonoMultiJack.Win.sln /p:Configuration=Release /t:Clean
call %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe MonoMultiJack.Win.sln /p:Configuration=Release

REM Packaging
CScript zip.vbs MonoMultiJack.Windows\bin\Release MonoMultiJack.zip
pause
