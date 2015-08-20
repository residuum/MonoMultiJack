SET VS_PATH="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe"
SET CONFIG=Release

CALL git submodule update --init
CALL %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe MonoMultiJack.Win.sln /p:Configuration=%CONFIG% /t:Clean
CALL %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe MonoMultiJack.Win.sln /p:Configuration=%CONFIG%

COPY MonoMultiJack.Windows.Setup\bin\%CONFIG%\MonoMultiJack.msi MonoMultiJack.msi

Pause
