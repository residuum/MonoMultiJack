
// This file has been generated by the GUI designer. Do not modify.
namespace MonoMultiJack
{
	public partial class ConnectionDisplay
	{
		private global::Gtk.HBox hbox1;

		private global::Gtk.ScrolledWindow GtkScrolledWindow;

		private global::Gtk.TreeView _outputTreeview;

		private global::Gtk.Fixed _connectionArea;

		private global::Gtk.ScrolledWindow GtkScrolledWindow1;

		private global::Gtk.TreeView _inputTreeview;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoMultiJack.ConnectionDisplay
			global::Stetic.BinContainer.Attach (this);
			this.WidthRequest = 500;
			this.HeightRequest = 200;
			this.Name = "MonoMultiJack.ConnectionDisplay";
			// Container child MonoMultiJack.ConnectionDisplay.Gtk.Container+ContainerChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this._outputTreeview = new global::Gtk.TreeView ();
			this._outputTreeview.CanFocus = true;
			this._outputTreeview.Name = "_outputTreeview";
			this._outputTreeview.EnableSearch = false;
			this._outputTreeview.HeadersVisible = false;
			this.GtkScrolledWindow.Add (this._outputTreeview);
			this.hbox1.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.GtkScrolledWindow]));
			w2.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this._connectionArea = new global::Gtk.Fixed ();
			this._connectionArea.WidthRequest = 200;
			this._connectionArea.Name = "_connectionArea";
			this._connectionArea.HasWindow = false;
			this.hbox1.Add (this._connectionArea);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1[this._connectionArea]));
			w3.Position = 1;
			// Container child hbox1.Gtk.Box+BoxChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this._inputTreeview = new global::Gtk.TreeView ();
			this._inputTreeview.CanFocus = true;
			this._inputTreeview.Name = "_inputTreeview";
			this._inputTreeview.EnableSearch = false;
			this._inputTreeview.HeadersVisible = false;
			this.GtkScrolledWindow1.Add (this._inputTreeview);
			this.hbox1.Add (this.GtkScrolledWindow1);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.GtkScrolledWindow1]));
			w5.Position = 2;
			this.Add (this.hbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Show ();
		}
	}
}
