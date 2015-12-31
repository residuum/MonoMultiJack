#!/bin/bash

XBUILD=xbuild
SOLUTION=MonoMultiJack.Lnx.Gtk2.sln
OTHER=MonoMultiJack.Lnx.Gtk3.sln

if [ "$1" == "Gtk3" ]; then
	SOLUTION=MonoMultiJack.Lnx.Gtk3.sln
	OTHER=MonoMultiJack.Lnx.Gtk2.sln
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
