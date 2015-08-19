#!/bin/bash

XBUILD=xbuild
SOLUTION=MonoMultiJack.Lnx.sln
OTHER=MonoMultiJack.Lnx.Gtk3.sln
cp -f MonoMultiJack.Linux/MonoMultiJack.Linux.config MonoMultiJack.Linux/app.config

if [ "$1" == "Gtk3" ]; then
	SOLUTION=MonoMultiJack.Lnx.Gtk3.sln
	OTHER=MonoMultiJack.Lnx.sln
	cp -f MonoMultiJack.Linux/MonoMultiJack.Linux.Gtk3.config MonoMultiJack.Linux/app.config
fi

git submodule update --init --recursive

${XBUILD} ${SOLUTION} /p:Configuration=Release /t:Clean
${XBUILD} ${OTHER} /p:Configuration=Release /t:Clean

${XBUILD} ${SOLUTION} /p:Configuration=Release

cd MonoMultiJack.Linux
bash Linux-pocompile.sh Release

cd bin/Release
tar -cf MonoMultiJack.tar *
bzip2 MonoMultiJack.tar
mv MonoMultiJack.tar.bz2 ../../../
