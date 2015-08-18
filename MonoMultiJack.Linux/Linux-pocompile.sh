#!/bin/bash

BASE_FOLDER=../MonoMultiJack.Linux/bin/$1/locale

for POFILE in ../Mmj.OS/*.po; do 
	read BASE_FILENAME LANG_CODE EXT <<<$(IFS="."; echo $POFILE)
	WRITE_DIR=${BASE_FOLDER}/${LANG_CODE}/LC_MESSAGES
	if [ ! -d "$WRITE_DIR" ]; then
		mkdir -p $WRITE_DIR
	fi
	msgfmt -o ${WRITE_DIR}/${BASE_FILENAME}.mo $POFILE
done
