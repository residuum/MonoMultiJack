
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/MonoMultiJack.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = winexe
PROJECT_REFERENCES =  \
	../MonoMultiJack.ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll \
	../MonoMultiJack.BusinessLogic/bin/Debug/MonoMultiJack.BusinessLogic.dll \
	../MonoMultiJack.ConnectionWrapper.Jack/bin/Debug/MonoMultiJack.ConnectionWrapper.Jack.dll
BUILD_DIR = bin/Debug

MONOMULTIJACK_CONNECTIONWRAPPER_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB_SOURCE=../MonoMultiJack.ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.dll.mdb
MONOMULTIJACK_BUSINESSLOGIC_DLL_SOURCE=../MonoMultiJack.BusinessLogic/bin/Debug/MonoMultiJack.BusinessLogic.dll
MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB_SOURCE=../MonoMultiJack.BusinessLogic/bin/Debug/MonoMultiJack.BusinessLogic.dll.mdb
MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.BusinessLogic.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper.Jack/bin/Debug/MonoMultiJack.ConnectionWrapper.Jack.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB_SOURCE=../MonoMultiJack.ConnectionWrapper.Jack/bin/Debug/MonoMultiJack.ConnectionWrapper.Jack.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Jack.dll.mdb
MONOMULTIJACK_EXE_MDB_SOURCE=bin/Debug/MonoMultiJack.exe.mdb
MONOMULTIJACK_EXE_MDB=$(BUILD_DIR)/MonoMultiJack.exe.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/MonoMultiJack.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = winexe
PROJECT_REFERENCES =  \
	../MonoMultiJack.ConnectionWrapper/bin/Release/MonoMultiJack.ConnectionWrapper.dll \
	../MonoMultiJack.BusinessLogic/bin/Release/MonoMultiJack.BusinessLogic.dll \
	../MonoMultiJack.ConnectionWrapper.Jack/bin/Release/MonoMultiJack.ConnectionWrapper.Jack.dll
BUILD_DIR = bin/Release

MONOMULTIJACK_CONNECTIONWRAPPER_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper/bin/Release/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB=
MONOMULTIJACK_BUSINESSLOGIC_DLL_SOURCE=../MonoMultiJack.BusinessLogic/bin/Release/MonoMultiJack.BusinessLogic.dll
MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB=
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper.Jack/bin/Release/MonoMultiJack.ConnectionWrapper.Jack.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB=
MONOMULTIJACK_EXE_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_DLL) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB) \
	$(MONOMULTIJACK_BUSINESSLOGIC_DLL) \
	$(MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB) \
	$(MONOMULTIJACK_EXE_MDB)  

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
	gtk-gui/MonoMultiJack.Widgets.ConnectionDisplay.cs 

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
	$(MONO_CAIRO_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

MONOMULTIJACK_CONNECTIONWRAPPER_DLL = $(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_BUSINESSLOGIC_DLL = $(BUILD_DIR)/MonoMultiJack.BusinessLogic.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL = $(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Jack.dll
MONOMULTIJACK = $(BUILD_DIR)/monomultijack

$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB))
$(eval $(call emit-deploy-target,MONOMULTIJACK_BUSINESSLOGIC_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB))
$(eval $(call emit-deploy-wrapper,MONOMULTIJACK,monomultijack,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
