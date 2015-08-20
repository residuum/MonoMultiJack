SETLOCAL EnableDelayedExpansion

SET GETTEXT_DIR=\Software\xgettext\bin
SET OUTPUT_BASE=%1\locale\
FOR %%f IN (%~dp0..\Mmj.Os\*.po) DO (
	ECHO %%f
	for %%T in (%%f) do (
		FOR /f "tokens=1,2,3 delims=." %%a in ("%%~nxT") DO (
			SET BASE_FILENAME=%%a
			SET FILE_LANG=%%b
			SET OUTPUT_DIR=%OUTPUT_BASE%%%b\LC_MESSAGES
			IF NOT EXIST !OUTPUT_DIR! (
				mkdir !OUTPUT_DIR!
			)
			CALL %GETTEXT_DIR%\msgfmt.exe -o !OUTPUT_DIR!\!BASE_FILENAME!.mo %%f
		)
	)
)
ENDLOCAL
