#!/bin/bash

XBUILD=xbuild
SOLUTION=MonoMultiJack.Lnx.sln
if [ "$1" == "Gtk3" ]; then
	SOLUTION=MonoMultiJack.Lnx.Gtk3.sln
fi
git submodule init
git submodule update
${XBUILD} ${SOLUTION} /p:Configuration=Release /t:Clean
#Not nice, but necessary because of https://bugzilla.xamarin.com/show_bug.cgi?id=18761
if [ "$1" == "Gtk3" ]; then
	cp -f MonoMultiJack.Linux/MonoMultiJack.Linux.Gtk3.config MonoMultiJack.Linux/app.config
else
	cp -f MonoMultiJack.Linux/MonoMultiJack.Linux.config MonoMultiJack.Linux/app.config
fi
${XBUILD} ${SOLUTION} /p:Configuration=Release
cd MonoMultiJack.Linux/bin/Release
tar -cf MonoMultiJack.tar *
bzip2 MonoMultiJack.tar
mv MonoMultiJack.tar.bz2 ../../../
