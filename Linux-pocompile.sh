#!/bin/bash

BASE_FOLDER=MonoMultiJack.Linux/bin/$1/locale

for POFILE in *.po; do 
	read BASE_FILENAME LANG EXT <<<$(IFS="."; echo $POFILE)
	WRITE_DIR=${BASE_FOLDER}/${LANG}/LC_MESSAGES
	if [ ! -d "$WRITE_DIR" ]; then
		mkdir -p $WRITE_DIR
	fi

	msgfmt -o ${WRITE_DIR}/${BASE_FILENAME}.mo $POFILE
done
