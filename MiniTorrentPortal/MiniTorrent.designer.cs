﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MiniTorrentPortal
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="MiniTorrentDB")]
	public partial class MiniTorrentDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertClientFile(ClientFile instance);
    partial void UpdateClientFile(ClientFile instance);
    partial void DeleteClientFile(ClientFile instance);
    partial void InsertFile(File instance);
    partial void UpdateFile(File instance);
    partial void DeleteFile(File instance);
    partial void InsertClients(Clients instance);
    partial void UpdateClients(Clients instance);
    partial void DeleteClients(Clients instance);
    #endregion
		
		public MiniTorrentDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["MiniTorrentDBConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public MiniTorrentDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public MiniTorrentDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public MiniTorrentDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public MiniTorrentDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<ClientFile> ClientFiles
		{
			get
			{
				return this.GetTable<ClientFile>();
			}
		}
		
		public System.Data.Linq.Table<File> Files
		{
			get
			{
				return this.GetTable<File>();
			}
		}
		
		public System.Data.Linq.Table<Clients> Clients
		{
			get
			{
				return this.GetTable<Clients>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.ClientFile")]
	public partial class ClientFile : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _Username;
		
		private string _FileName;
		
		private EntityRef<File> _File;
		
		private EntityRef<Clients> _Clients;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnUsernameChanging(string value);
    partial void OnUsernameChanged();
    partial void OnFileNameChanging(string value);
    partial void OnFileNameChanged();
    #endregion
		
		public ClientFile()
		{
			this._File = default(EntityRef<File>);
			this._Clients = default(EntityRef<Clients>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Username", DbType="NVarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string Username
		{
			get
			{
				return this._Username;
			}
			set
			{
				if ((this._Username != value))
				{
					if (this._Clients.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnUsernameChanging(value);
					this.SendPropertyChanging();
					this._Username = value;
					this.SendPropertyChanged("Username");
					this.OnUsernameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileName", DbType="NVarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string FileName
		{
			get
			{
				return this._FileName;
			}
			set
			{
				if ((this._FileName != value))
				{
					if (this._File.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnFileNameChanging(value);
					this.SendPropertyChanging();
					this._FileName = value;
					this.SendPropertyChanged("FileName");
					this.OnFileNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="File_ClientFile", Storage="_File", ThisKey="FileName", OtherKey="Name", IsForeignKey=true)]
		public File File
		{
			get
			{
				return this._File.Entity;
			}
			set
			{
				File previousValue = this._File.Entity;
				if (((previousValue != value) 
							|| (this._File.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._File.Entity = null;
						previousValue.ClientFiles.Remove(this);
					}
					this._File.Entity = value;
					if ((value != null))
					{
						value.ClientFiles.Add(this);
						this._FileName = value.Name;
					}
					else
					{
						this._FileName = default(string);
					}
					this.SendPropertyChanged("File");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Client_ClientFile", Storage="_Clients", ThisKey="Username", OtherKey="Username", IsForeignKey=true)]
		public Clients Clients
		{
			get
			{
				return this._Clients.Entity;
			}
			set
			{
				Clients previousValue = this._Clients.Entity;
				if (((previousValue != value) 
							|| (this._Clients.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Clients.Entity = null;
						previousValue.ClientFiles.Remove(this);
					}
					this._Clients.Entity = value;
					if ((value != null))
					{
						value.ClientFiles.Add(this);
						this._Username = value.Username;
					}
					else
					{
						this._Username = default(string);
					}
					this.SendPropertyChanged("Clients");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Files")]
	public partial class File : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _Name;
		
		private int _Size;
		
		private EntitySet<ClientFile> _ClientFiles;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnSizeChanging(int value);
    partial void OnSizeChanged();
    #endregion
		
		public File()
		{
			this._ClientFiles = new EntitySet<ClientFile>(new Action<ClientFile>(this.attach_ClientFiles), new Action<ClientFile>(this.detach_ClientFiles));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Size", DbType="Int NOT NULL")]
		public int Size
		{
			get
			{
				return this._Size;
			}
			set
			{
				if ((this._Size != value))
				{
					this.OnSizeChanging(value);
					this.SendPropertyChanging();
					this._Size = value;
					this.SendPropertyChanged("Size");
					this.OnSizeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="File_ClientFile", Storage="_ClientFiles", ThisKey="Name", OtherKey="FileName")]
		public EntitySet<ClientFile> ClientFiles
		{
			get
			{
				return this._ClientFiles;
			}
			set
			{
				this._ClientFiles.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_ClientFiles(ClientFile entity)
		{
			this.SendPropertyChanging();
			entity.File = this;
		}
		
		private void detach_ClientFiles(ClientFile entity)
		{
			this.SendPropertyChanging();
			entity.File = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Clients")]
	public partial class Clients : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _Username;
		
		private string _Password;
		
		private string _UpPath;
		
		private string _DownPath;
		
		private bool _Active;
		
		private bool _Admin;
		
		private string _IP;
		
		private System.Nullable<int> _Port;
		
		private EntitySet<ClientFile> _ClientFiles;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnUsernameChanging(string value);
    partial void OnUsernameChanged();
    partial void OnPasswordChanging(string value);
    partial void OnPasswordChanged();
    partial void OnUpPathChanging(string value);
    partial void OnUpPathChanged();
    partial void OnDownPathChanging(string value);
    partial void OnDownPathChanged();
    partial void OnActiveChanging(bool value);
    partial void OnActiveChanged();
    partial void OnAdminChanging(bool value);
    partial void OnAdminChanged();
    partial void OnIPChanging(string value);
    partial void OnIPChanged();
    partial void OnPortChanging(System.Nullable<int> value);
    partial void OnPortChanged();
    #endregion
		
		public Clients()
		{
			this._ClientFiles = new EntitySet<ClientFile>(new Action<ClientFile>(this.attach_ClientFiles), new Action<ClientFile>(this.detach_ClientFiles));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Username", DbType="NVarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string Username
		{
			get
			{
				return this._Username;
			}
			set
			{
				if ((this._Username != value))
				{
					this.OnUsernameChanging(value);
					this.SendPropertyChanging();
					this._Username = value;
					this.SendPropertyChanged("Username");
					this.OnUsernameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Password", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Password
		{
			get
			{
				return this._Password;
			}
			set
			{
				if ((this._Password != value))
				{
					this.OnPasswordChanging(value);
					this.SendPropertyChanging();
					this._Password = value;
					this.SendPropertyChanged("Password");
					this.OnPasswordChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UpPath", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string UpPath
		{
			get
			{
				return this._UpPath;
			}
			set
			{
				if ((this._UpPath != value))
				{
					this.OnUpPathChanging(value);
					this.SendPropertyChanging();
					this._UpPath = value;
					this.SendPropertyChanged("UpPath");
					this.OnUpPathChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DownPath", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string DownPath
		{
			get
			{
				return this._DownPath;
			}
			set
			{
				if ((this._DownPath != value))
				{
					this.OnDownPathChanging(value);
					this.SendPropertyChanging();
					this._DownPath = value;
					this.SendPropertyChanged("DownPath");
					this.OnDownPathChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Active", DbType="Bit NOT NULL")]
		public bool Active
		{
			get
			{
				return this._Active;
			}
			set
			{
				if ((this._Active != value))
				{
					this.OnActiveChanging(value);
					this.SendPropertyChanging();
					this._Active = value;
					this.SendPropertyChanged("Active");
					this.OnActiveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Admin", DbType="Bit NOT NULL")]
		public bool Admin
		{
			get
			{
				return this._Admin;
			}
			set
			{
				if ((this._Admin != value))
				{
					this.OnAdminChanging(value);
					this.SendPropertyChanging();
					this._Admin = value;
					this.SendPropertyChanged("Admin");
					this.OnAdminChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IP", DbType="NVarChar(50)")]
		public string IP
		{
			get
			{
				return this._IP;
			}
			set
			{
				if ((this._IP != value))
				{
					this.OnIPChanging(value);
					this.SendPropertyChanging();
					this._IP = value;
					this.SendPropertyChanged("IP");
					this.OnIPChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Port", DbType="Int")]
		public System.Nullable<int> Port
		{
			get
			{
				return this._Port;
			}
			set
			{
				if ((this._Port != value))
				{
					this.OnPortChanging(value);
					this.SendPropertyChanging();
					this._Port = value;
					this.SendPropertyChanged("Port");
					this.OnPortChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Client_ClientFile", Storage="_ClientFiles", ThisKey="Username", OtherKey="Username")]
		public EntitySet<ClientFile> ClientFiles
		{
			get
			{
				return this._ClientFiles;
			}
			set
			{
				this._ClientFiles.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_ClientFiles(ClientFile entity)
		{
			this.SendPropertyChanging();
			entity.Clients = this;
		}
		
		private void detach_ClientFiles(ClientFile entity)
		{
			this.SendPropertyChanging();
			entity.Clients = null;
		}
	}
}
#pragma warning restore 1591
