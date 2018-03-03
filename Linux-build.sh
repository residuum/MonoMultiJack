#!/bin/bash

XBUILD=xbuild
SOLUTION=MonoMultiJack.Lnx.sln

git submodule update --init --recursive

${XBUILD} ${SOLUTION} /p:Configuration=Release /t:Clean

${XBUILD} ${SOLUTION} /p:Configuration=Release

cd MonoMultiJack.Linux
bash Linux-pocompile.sh Release

cd bin/Release
tar -cf MonoMultiJack.tar *
bzip2 MonoMultiJack.tar
mv MonoMultiJack.tar.bz2 ../../../
