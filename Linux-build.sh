#!/bin/sh
git submodule init
git submodule update
xbuild MonoMultiJack.Lnx.sln /t:Clean
xbuild MonoMultiJack.Lnx.sln /p:Configuration=Release
cd MonoMultiJack.Linux/bin/Release
tar -cf MonoMultiJack.tar *
bzip2 MonoMultiJack.tar
mv MonoMultiJack.tar.bz2 ../../../
