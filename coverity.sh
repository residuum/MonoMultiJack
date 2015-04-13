#!/bin/bash
$WINDIR/Microsoft.NET/Framework/v4.0.30319/msbuild.exe MonoMultiJack.Win.sln -p:Configuration=Release -t:Clean
cov-build --dir cov-int $WINDIR/Microsoft.NET/Framework/v4.0.30319/msbuild.exe MonoMultiJack.Win.sln -p:Configuration=Release -t:Rebuild
