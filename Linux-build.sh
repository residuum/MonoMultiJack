#!/bin/sh
git submodule init
git submodule update
xbuild MonoMultiJack.Lnx.sln /p:Configuration=Release
