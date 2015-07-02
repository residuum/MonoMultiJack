SET VS_PATH="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe"
SET CONFIG=Release

CALL git submodule update --init
CALL %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe MonoMultiJack.Win.sln /p:Configuration=%CONFIG% /t:Clean
CALL %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe MonoMultiJack.Win.sln /p:Configuration=%CONFIG%
CALL %VS_PATH% MonoMultiJack.Win.sln /build "%CONFIG%" /project MonoMultiJack.Windows.Setup\MonoMultiJack.Windows.Setup.vdproj /projectconfig "%CONFIG%"

COPY MonoMultiJack.Windows.Setup\%CONFIG%\MonoMultiJack.Setup.msi MonoMultiJack.Setup.msi
COPY MonoMultiJack.Windows.Setup\%CONFIG%\setup.exe MonoMultiJack-setup.exe

Pause
