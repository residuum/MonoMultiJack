
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/MonoMultiJack.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = winexe
PROJECT_REFERENCES =  \
	ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll \
	BusinessLogic/bin/Debug/MonoMultiJack.BusinessLogic.dll \
	ConnectionWrapper.Jack/bin/Debug/MonoMultiJack.ConnectionWrapper.Jack.dll \
	ConnectionWrapper.Alsa/bin/Debug/MonoMultiJack.ConnectionWrapper.Alsa.dll
BUILD_DIR = bin/Debug

MONOMULTIJACK_EXE_MDB_SOURCE=bin/Debug/MonoMultiJack.exe.mdb
MONOMULTIJACK_EXE_MDB=$(BUILD_DIR)/MonoMultiJack.exe.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_SOURCE=ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB_SOURCE=ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.dll.mdb
MONOMULTIJACK_BUSINESSLOGIC_DLL_SOURCE=BusinessLogic/bin/Debug/MonoMultiJack.BusinessLogic.dll
MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB_SOURCE=BusinessLogic/bin/Debug/MonoMultiJack.BusinessLogic.dll.mdb
MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.BusinessLogic.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_SOURCE=ConnectionWrapper.Jack/bin/Debug/MonoMultiJack.ConnectionWrapper.Jack.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB_SOURCE=ConnectionWrapper.Jack/bin/Debug/MonoMultiJack.ConnectionWrapper.Jack.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Jack.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_SOURCE=ConnectionWrapper.Alsa/bin/Debug/MonoMultiJack.ConnectionWrapper.Alsa.dll
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB_SOURCE=ConnectionWrapper.Alsa/bin/Debug/MonoMultiJack.ConnectionWrapper.Alsa.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Alsa.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/MonoMultiJack.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = winexe
PROJECT_REFERENCES =  \
	ConnectionWrapper/bin/Release/MonoMultiJack.ConnectionWrapper.dll \
	BusinessLogic/bin/Release/MonoMultiJack.BusinessLogic.dll \
	ConnectionWrapper.Jack/bin/Release/MonoMultiJack.ConnectionWrapper.Jack.dll \
	ConnectionWrapper.Alsa/bin/Release/MonoMultiJack.ConnectionWrapper.Alsa.dll
BUILD_DIR = bin/Release

MONOMULTIJACK_EXE_MDB=
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_SOURCE=ConnectionWrapper/bin/Release/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB=
MONOMULTIJACK_BUSINESSLOGIC_DLL_SOURCE=BusinessLogic/bin/Release/MonoMultiJack.BusinessLogic.dll
MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB=
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_SOURCE=ConnectionWrapper.Jack/bin/Release/MonoMultiJack.ConnectionWrapper.Jack.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB=
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_SOURCE=ConnectionWrapper.Alsa/bin/Release/MonoMultiJack.ConnectionWrapper.Alsa.dll
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB=

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONOMULTIJACK_EXE_MDB) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_DLL) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB) \
	$(MONOMULTIJACK_BUSINESSLOGIC_DLL) \
	$(MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB)  

BINARIES = \
	$(MONOMULTIJACK)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES) 

FILES = \
	Main.cs \
	AssemblyInfo.cs \
	Forms/MainWindow.cs \
	Forms/AppConfigWindow.cs \
	Forms/JackdConfigWindow.cs \
	Widgets/AppWidget.cs \
	Widgets/AppConfigWidget.cs \
	gtk-gui/generated.cs \
	Widgets/ConnectionDisplay.cs \
	gtk-gui/MonoMultiJack.Forms.MainWindow.cs \
	gtk-gui/MonoMultiJack.Widgets.ConnectionDisplay.cs \
	gtk-gui/MonoMultiJack.Forms.AppConfigWindow.cs 

DATA_FILES = 

RESOURCES = \
	gtk-gui/gui.stetic 

EXTRAS = \
	Forms \
	Widgets \
	monomultijack.in 

REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS) \
	$(GLADE_SHARP_20_LIBS) \
	Mono.Posix \
	System.Xml \
	System.Core \
	System \
	Mono.Cairo

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

MONOMULTIJACK_CONNECTIONWRAPPER_DLL = $(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_BUSINESSLOGIC_DLL = $(BUILD_DIR)/MonoMultiJack.BusinessLogic.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL = $(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Jack.dll
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL = $(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Alsa.dll
MONOMULTIJACK = $(BUILD_DIR)/monomultijack

$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB))
$(eval $(call emit-deploy-target,MONOMULTIJACK_BUSINESSLOGIC_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB))
$(eval $(call emit-deploy-wrapper,MONOMULTIJACK,monomultijack,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
