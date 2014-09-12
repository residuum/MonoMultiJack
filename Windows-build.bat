REM Building
call git submodule init
call git submodule update
call %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe MonoMultiJack.Win.sln /t:Clean
call %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe MonoMultiJack.Win.sln /p:Configuration=Release

REM Packaging
CScript zip.vbs MonoMultiJack.Windows\bin\Release MonoMultiJack.zip
pause
