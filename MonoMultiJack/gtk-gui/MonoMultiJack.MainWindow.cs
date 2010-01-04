// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoMultiJack {
    
    
    public partial class MainWindow {
        
        private Gtk.UIManager UIManager;
        
        private Gtk.Action fileAction;
        
        private Gtk.Action reStartJackdAction;
        
        private Gtk.Action stopJackdAction;
        
        private Gtk.Action stopAllAction;
        
        private Gtk.Action quitAction;
        
        private Gtk.Action ConfigurationAction;
        
        private Gtk.Action configureJackdAction;
        
        private Gtk.Action addRemoveApplicationsAction;
        
        private Gtk.Action helpAction;
        
        private Gtk.Action aboutAction;
        
        private Gtk.VBox mainVbox;
        
        private Gtk.MenuBar menubar1;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget MonoMultiJack.MainWindow
            this.UIManager = new Gtk.UIManager();
            Gtk.ActionGroup w1 = new Gtk.ActionGroup("Default");
            this.fileAction = new Gtk.Action("fileAction", Mono.Unix.Catalog.GetString("File"), null, null);
            this.fileAction.ShortLabel = Mono.Unix.Catalog.GetString("File");
            w1.Add(this.fileAction, null);
            this.reStartJackdAction = new Gtk.Action("reStartJackdAction", Mono.Unix.Catalog.GetString("(Re)Start Jackd"), null, "gtk-refresh");
            this.reStartJackdAction.Sensitive = false;
            this.reStartJackdAction.ShortLabel = Mono.Unix.Catalog.GetString("(Re)Start Jackd");
            w1.Add(this.reStartJackdAction, null);
            this.stopJackdAction = new Gtk.Action("stopJackdAction", Mono.Unix.Catalog.GetString("Stop Jackd"), null, "gtk-stop");
            this.stopJackdAction.Sensitive = false;
            this.stopJackdAction.ShortLabel = Mono.Unix.Catalog.GetString("Stop Jackd");
            w1.Add(this.stopJackdAction, null);
            this.stopAllAction = new Gtk.Action("stopAllAction", Mono.Unix.Catalog.GetString("Stop All"), null, "gtk-stop");
            this.stopAllAction.Sensitive = false;
            this.stopAllAction.ShortLabel = Mono.Unix.Catalog.GetString("Stop All");
            w1.Add(this.stopAllAction, null);
            this.quitAction = new Gtk.Action("quitAction", Mono.Unix.Catalog.GetString("Quit"), null, "gtk-quit");
            this.quitAction.ShortLabel = Mono.Unix.Catalog.GetString("Quit");
            w1.Add(this.quitAction, null);
            this.ConfigurationAction = new Gtk.Action("ConfigurationAction", Mono.Unix.Catalog.GetString("Configuration"), null, null);
            this.ConfigurationAction.ShortLabel = Mono.Unix.Catalog.GetString("Configuration");
            w1.Add(this.ConfigurationAction, null);
            this.configureJackdAction = new Gtk.Action("configureJackdAction", Mono.Unix.Catalog.GetString("Configure Jackd"), null, "gtk-preferences");
            this.configureJackdAction.ShortLabel = Mono.Unix.Catalog.GetString("Configure Jackd");
            w1.Add(this.configureJackdAction, null);
            this.addRemoveApplicationsAction = new Gtk.Action("addRemoveApplicationsAction", Mono.Unix.Catalog.GetString("Add / Remove Applications"), null, "gtk-preferences");
            this.addRemoveApplicationsAction.ShortLabel = Mono.Unix.Catalog.GetString("Add / Remove Applications");
            w1.Add(this.addRemoveApplicationsAction, null);
            this.helpAction = new Gtk.Action("helpAction", Mono.Unix.Catalog.GetString("Help"), null, null);
            this.helpAction.ShortLabel = Mono.Unix.Catalog.GetString("Help");
            w1.Add(this.helpAction, null);
            this.aboutAction = new Gtk.Action("aboutAction", Mono.Unix.Catalog.GetString("About"), null, "gtk-about");
            this.aboutAction.ShortLabel = Mono.Unix.Catalog.GetString("About");
            w1.Add(this.aboutAction, null);
            this.UIManager.InsertActionGroup(w1, 0);
            this.AddAccelGroup(this.UIManager.AccelGroup);
            this.Name = "MonoMultiJack.MainWindow";
            this.Title = Mono.Unix.Catalog.GetString("MainWindow");
            this.WindowPosition = ((Gtk.WindowPosition)(4));
            // Container child MonoMultiJack.MainWindow.Gtk.Container+ContainerChild
            this.mainVbox = new Gtk.VBox();
            this.mainVbox.Name = "mainVbox";
            // Container child mainVbox.Gtk.Box+BoxChild
            this.UIManager.AddUiFromString("<ui><menubar name='menubar1'><menu name='fileAction' action='fileAction'><menuitem name='reStartJackdAction' action='reStartJackdAction'/><menuitem name='stopJackdAction' action='stopJackdAction'/><menuitem name='stopAllAction' action='stopAllAction'/><menuitem name='quitAction' action='quitAction'/></menu><menu name='ConfigurationAction' action='ConfigurationAction'><menuitem name='configureJackdAction' action='configureJackdAction'/><menuitem name='addRemoveApplicationsAction' action='addRemoveApplicationsAction'/></menu><menu name='helpAction' action='helpAction'><menuitem name='aboutAction' action='aboutAction'/></menu></menubar></ui>");
            this.menubar1 = ((Gtk.MenuBar)(this.UIManager.GetWidget("/menubar1")));
            this.menubar1.Name = "menubar1";
            this.mainVbox.Add(this.menubar1);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.mainVbox[this.menubar1]));
            w2.Position = 0;
            w2.Expand = false;
            w2.Fill = false;
            this.Add(this.mainVbox);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 275;
            this.DefaultHeight = 175;
            this.Show();
            this.reStartJackdAction.Activated += new System.EventHandler(this.RestartJackd);
            this.stopJackdAction.Activated += new System.EventHandler(this.StopJackd);
            this.stopAllAction.Activated += new System.EventHandler(this.StopAll);
            this.quitAction.Activated += new System.EventHandler(this.OnQuitActionActivated);
            this.configureJackdAction.Activated += new System.EventHandler(this.ConfigureJackd);
            this.addRemoveApplicationsAction.Activated += new System.EventHandler(this.ConfigureApplications);
            this.aboutAction.Activated += new System.EventHandler(this.AboutDialog);
        }
    }
}