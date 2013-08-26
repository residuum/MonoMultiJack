
// This file has been generated by the GUI designer. Do not modify.
namespace MonoMultiJack.Widgets
{
	public partial class ConnectionDisplay
	{
		private global::Gtk.VBox vbox1;
		private global::Gtk.HBox hbox2;
		private global::Gtk.HBox hbox3;
		private global::Gtk.Button _connectButton;
		private global::Gtk.Button _disconnectButton;
		private global::Gtk.HBox hbox4;
		private global::Gtk.ScrolledWindow _outputScrolledWindow;
		private global::Gtk.TreeView _outputTreeview;
		private global::Gtk.DrawingArea _connectionArea;
		private global::Gtk.ScrolledWindow _inputScrolledWindow;
		private global::Gtk.TreeView _inputTreeview;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoMultiJack.Widgets.ConnectionDisplay
			global::Stetic.BinContainer.Attach (this);
			this.WidthRequest = 500;
			this.HeightRequest = 200;
			this.Name = "MonoMultiJack.Widgets.ConnectionDisplay";
			// Container child MonoMultiJack.Widgets.ConnectionDisplay.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this._connectButton = new global::Gtk.Button ();
			this._connectButton.CanFocus = true;
			this._connectButton.Name = "_connectButton";
			this._connectButton.UseUnderline = true;
			this._connectButton.Label = global::Mono.Unix.Catalog.GetString ("Connect");
			this.hbox3.Add (this._connectButton);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this._connectButton]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this._disconnectButton = new global::Gtk.Button ();
			this._disconnectButton.CanFocus = true;
			this._disconnectButton.Name = "_disconnectButton";
			this._disconnectButton.UseUnderline = true;
			this._disconnectButton.Label = global::Mono.Unix.Catalog.GetString ("Disconnect");
			this.hbox3.Add (this._disconnectButton);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this._disconnectButton]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			this.hbox2.Add (this.hbox3);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.hbox3]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			this.vbox1.Add (this.hbox2);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox2]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox4 = new global::Gtk.HBox ();
			this.hbox4.Name = "hbox4";
			this.hbox4.Spacing = 6;
			// Container child hbox4.Gtk.Box+BoxChild
			this._outputScrolledWindow = new global::Gtk.ScrolledWindow ();
			this._outputScrolledWindow.Name = "_outputScrolledWindow";
			this._outputScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child _outputScrolledWindow.Gtk.Container+ContainerChild
			this._outputTreeview = new global::Gtk.TreeView ();
			this._outputTreeview.CanFocus = true;
			this._outputTreeview.Name = "_outputTreeview";
			this._outputTreeview.EnableSearch = false;
			this._outputTreeview.HeadersVisible = false;
			this._outputScrolledWindow.Add (this._outputTreeview);
			this.hbox4.Add (this._outputScrolledWindow);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this._outputScrolledWindow]));
			w6.Position = 0;
			// Container child hbox4.Gtk.Box+BoxChild
			this._connectionArea = new global::Gtk.DrawingArea ();
			this._connectionArea.WidthRequest = 200;
			this._connectionArea.Name = "_connectionArea";
			this.hbox4.Add (this._connectionArea);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this._connectionArea]));
			w7.Position = 1;
			// Container child hbox4.Gtk.Box+BoxChild
			this._inputScrolledWindow = new global::Gtk.ScrolledWindow ();
			this._inputScrolledWindow.Name = "_inputScrolledWindow";
			this._inputScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child _inputScrolledWindow.Gtk.Container+ContainerChild
			this._inputTreeview = new global::Gtk.TreeView ();
			this._inputTreeview.CanFocus = true;
			this._inputTreeview.Name = "_inputTreeview";
			this._inputTreeview.EnableSearch = false;
			this._inputTreeview.HeadersVisible = false;
			this._inputScrolledWindow.Add (this._inputTreeview);
			this.hbox4.Add (this._inputScrolledWindow);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this._inputScrolledWindow]));
			w9.Position = 2;
			this.vbox1.Add (this.hbox4);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox4]));
			w10.Position = 1;
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Show ();
			this.ExposeEvent += new global::Gtk.ExposeEventHandler (this.Handle_ExposeEvent);
			this._connectButton.Clicked += new global::System.EventHandler (this.ConnectButton_Click);
			this._disconnectButton.Clicked += new global::System.EventHandler (this.DisconnectButton_Click);
			this._outputTreeview.RowExpanded += new global::Gtk.RowExpandedHandler (this.OnTreeViewRowExpanded);
			this._outputTreeview.RowCollapsed += new global::Gtk.RowCollapsedHandler (this.OnTreeViewRowCollapsed);
			this._inputTreeview.RowExpanded += new global::Gtk.RowExpandedHandler (this.OnTreeViewRowExpanded);
			this._inputTreeview.RowCollapsed += new global::Gtk.RowCollapsedHandler (this.OnTreeViewRowCollapsed);
		}
	}
}
