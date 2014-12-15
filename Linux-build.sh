#!/bin/bash
SOLUTION=MonoMultiJack.Lnx.sln
if [ "$1" == "Gtk3" ]; then
	SOLUTION=MonoMultiJack.Lnx.Gtk3.sln
fi
git submodule init
git submodule update
xbuild $SOLUTION /p:Configuration=Release /t:Clean
xbuild $SOLUTION /p:Configuration=Release
cd MonoMultiJack.Linux/bin/Release
tar -cf MonoMultiJack.tar *
bzip2 MonoMultiJack.tar
mv MonoMultiJack.tar.bz2 ../../../
