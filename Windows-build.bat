SET CONFIG=Release

CALL git submodule update --init
CALL %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe MonoMultiJack.Win.sln /p:Configuration=%CONFIG% /t:Clean
CALL %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe MonoMultiJack.Win.sln /p:Configuration=%CONFIG%

COPY MonoMultiJack.Windows.Setup\bin\%CONFIG%\MonoMultiJack.msi MonoMultiJack.msi

Pause
