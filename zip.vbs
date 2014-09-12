folder = WScript.Arguments(0)
zipFile = WScript.Arguments(1)

Set fileSystem = CreateObject("Scripting.FileSystemObject")
Set shellApplication = CreateObject("Shell.Application")
Set source = shellApplication.NameSpace(fileSystem.GetAbsolutePathName(folder)).Items

fileSystem.CreateTextFile(zipFile, True).Write "PK" & Chr(5) & Chr(6) & String(18, vbNullChar)
shellApplication.NameSpace(fileSystem.GetAbsolutePathName(zipFile)).CopyHere(source)

wScript.Sleep 2000
