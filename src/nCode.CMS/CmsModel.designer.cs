﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace nCode.CMS
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
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="ncode_dk")]
	public partial class CmsModel : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertContentBlock(ContentBlock instance);
    partial void UpdateContentBlock(ContentBlock instance);
    partial void DeleteContentBlock(ContentBlock instance);
    partial void InsertContentBlockLocalization(ContentBlockLocalization instance);
    partial void UpdateContentBlockLocalization(ContentBlockLocalization instance);
    partial void DeleteContentBlockLocalization(ContentBlockLocalization instance);
    partial void InsertNews(News instance);
    partial void UpdateNews(News instance);
    partial void DeleteNews(News instance);
    partial void InsertNewsGroup(NewsGroup instance);
    partial void UpdateNewsGroup(NewsGroup instance);
    partial void DeleteNewsGroup(NewsGroup instance);
    partial void InsertNewsInNewsGroup(NewsInNewsGroup instance);
    partial void UpdateNewsInNewsGroup(NewsInNewsGroup instance);
    partial void DeleteNewsInNewsGroup(NewsInNewsGroup instance);
    #endregion
		
		public CmsModel(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public CmsModel(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public CmsModel(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public CmsModel(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<ContentBlock> ContentBlocks
		{
			get
			{
				return this.GetTable<ContentBlock>();
			}
		}
		
		public System.Data.Linq.Table<ContentBlockLocalization> ContentBlockLocalizations
		{
			get
			{
				return this.GetTable<ContentBlockLocalization>();
			}
		}
		
		public System.Data.Linq.Table<News> News
		{
			get
			{
				return this.GetTable<News>();
			}
		}
		
		public System.Data.Linq.Table<NewsGroup> NewsGroups
		{
			get
			{
				return this.GetTable<NewsGroup>();
			}
		}
		
		public System.Data.Linq.Table<NewsInNewsGroup> NewsInNewsGroups
		{
			get
			{
				return this.GetTable<NewsInNewsGroup>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="CMS_ContentBlock")]
	public partial class ContentBlock : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _ID;
		
		private System.DateTime _Created;
		
		private System.DateTime _Modified;
		
		private string _Name;
		
		private EntitySet<ContentBlockLocalization> _Localizations;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(System.Guid value);
    partial void OnIDChanged();
    partial void OnCreatedChanging(System.DateTime value);
    partial void OnCreatedChanged();
    partial void OnModifiedChanging(System.DateTime value);
    partial void OnModifiedChanged();
    partial void OnCodeChanging(string value);
    partial void OnCodeChanged();
    #endregion
		
		public ContentBlock()
		{
			this._Localizations = new EntitySet<ContentBlockLocalization>(new Action<ContentBlockLocalization>(this.attach_Localizations), new Action<ContentBlockLocalization>(this.detach_Localizations));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Created", DbType="DateTime NOT NULL")]
		public System.DateTime Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				if ((this._Created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Modified", DbType="DateTime NOT NULL")]
		public System.DateTime Modified
		{
			get
			{
				return this._Modified;
			}
			set
			{
				if ((this._Modified != value))
				{
					this.OnModifiedChanging(value);
					this.SendPropertyChanging();
					this._Modified = value;
					this.SendPropertyChanged("Modified");
					this.OnModifiedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string Code
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnCodeChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Code");
					this.OnCodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="ContentBlock_ContentBlockLocalization", Storage="_Localizations", ThisKey="ID", OtherKey="ContentBlockID")]
		public EntitySet<ContentBlockLocalization> Localizations
		{
			get
			{
				return this._Localizations;
			}
			set
			{
				this._Localizations.Assign(value);
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
		
		private void attach_Localizations(ContentBlockLocalization entity)
		{
			this.SendPropertyChanging();
			entity.ContentBlock = this;
		}
		
		private void detach_Localizations(ContentBlockLocalization entity)
		{
			this.SendPropertyChanging();
			entity.ContentBlock = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="CMS_ContentBlockLocalization")]
	public partial class ContentBlockLocalization : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _ID;
		
		private System.Guid _ContentBlockID;
		
		private string _Culture;
		
		private string _Content;
		
		private EntityRef<ContentBlock> _ContentBlock;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(System.Guid value);
    partial void OnIDChanged();
    partial void OnContentBlockIDChanging(System.Guid value);
    partial void OnContentBlockIDChanged();
    partial void OnCultureChanging(string value);
    partial void OnCultureChanged();
    partial void OnContentChanging(string value);
    partial void OnContentChanged();
    #endregion
		
		public ContentBlockLocalization()
		{
			this._ContentBlock = default(EntityRef<ContentBlock>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ContentBlockID", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid ContentBlockID
		{
			get
			{
				return this._ContentBlockID;
			}
			set
			{
				if ((this._ContentBlockID != value))
				{
					if (this._ContentBlock.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnContentBlockIDChanging(value);
					this.SendPropertyChanging();
					this._ContentBlockID = value;
					this.SendPropertyChanged("ContentBlockID");
					this.OnContentBlockIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Culture", DbType="NVarChar(255)")]
		public string Culture
		{
			get
			{
				return this._Culture;
			}
			set
			{
				if ((this._Culture != value))
				{
					this.OnCultureChanging(value);
					this.SendPropertyChanging();
					this._Culture = value;
					this.SendPropertyChanged("Culture");
					this.OnCultureChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Content", DbType="NText", UpdateCheck=UpdateCheck.Never)]
		public string Content
		{
			get
			{
				return this._Content;
			}
			set
			{
				if ((this._Content != value))
				{
					this.OnContentChanging(value);
					this.SendPropertyChanging();
					this._Content = value;
					this.SendPropertyChanged("Content");
					this.OnContentChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="ContentBlock_ContentBlockLocalization", Storage="_ContentBlock", ThisKey="ContentBlockID", OtherKey="ID", IsForeignKey=true)]
		public ContentBlock ContentBlock
		{
			get
			{
				return this._ContentBlock.Entity;
			}
			set
			{
				ContentBlock previousValue = this._ContentBlock.Entity;
				if (((previousValue != value) 
							|| (this._ContentBlock.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._ContentBlock.Entity = null;
						previousValue.Localizations.Remove(this);
					}
					this._ContentBlock.Entity = value;
					if ((value != null))
					{
						value.Localizations.Add(this);
						this._ContentBlockID = value.ID;
					}
					else
					{
						this._ContentBlockID = default(System.Guid);
					}
					this.SendPropertyChanged("ContentBlock");
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
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.CMS_News")]
	public partial class News : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _ID;
		
		private System.DateTime _Created;
		
		private System.DateTime _Modified;
		
		private string _Culture;
		
		private string _Title;
		
		private string _Introduction;
		
		private string _Text;
		
		private System.DateTime _ValidFrom;
		
		private System.Nullable<System.DateTime> _ValidTo;
		
		private string _Image;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(System.Guid value);
    partial void OnIDChanged();
    partial void OnCreatedChanging(System.DateTime value);
    partial void OnCreatedChanged();
    partial void OnModifiedChanging(System.DateTime value);
    partial void OnModifiedChanged();
    partial void OnCultureChanging(string value);
    partial void OnCultureChanged();
    partial void OnTitleChanging(string value);
    partial void OnTitleChanged();
    partial void OnIntroductionChanging(string value);
    partial void OnIntroductionChanged();
    partial void OnTextChanging(string value);
    partial void OnTextChanged();
    partial void OnValidFromChanging(System.DateTime value);
    partial void OnValidFromChanged();
    partial void OnValidToChanging(System.Nullable<System.DateTime> value);
    partial void OnValidToChanged();
    partial void OnImageChanging(string value);
    partial void OnImageChanged();
    #endregion
		
		public News()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Created", DbType="DateTime NOT NULL")]
		public System.DateTime Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				if ((this._Created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Modified", DbType="DateTime NOT NULL")]
		public System.DateTime Modified
		{
			get
			{
				return this._Modified;
			}
			set
			{
				if ((this._Modified != value))
				{
					this.OnModifiedChanging(value);
					this.SendPropertyChanging();
					this._Modified = value;
					this.SendPropertyChanged("Modified");
					this.OnModifiedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="Language", Storage="_Culture", DbType="NVarChar(10)")]
		public string Culture
		{
			get
			{
				return this._Culture;
			}
			set
			{
				if ((this._Culture != value))
				{
					this.OnCultureChanging(value);
					this.SendPropertyChanging();
					this._Culture = value;
					this.SendPropertyChanged("Culture");
					this.OnCultureChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Title", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string Title
		{
			get
			{
				return this._Title;
			}
			set
			{
				if ((this._Title != value))
				{
					this.OnTitleChanging(value);
					this.SendPropertyChanging();
					this._Title = value;
					this.SendPropertyChanged("Title");
					this.OnTitleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Introduction", DbType="NText", UpdateCheck=UpdateCheck.Never)]
		public string Introduction
		{
			get
			{
				return this._Introduction;
			}
			set
			{
				if ((this._Introduction != value))
				{
					this.OnIntroductionChanging(value);
					this.SendPropertyChanging();
					this._Introduction = value;
					this.SendPropertyChanged("Introduction");
					this.OnIntroductionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Text", DbType="NText", UpdateCheck=UpdateCheck.Never)]
		public string Text
		{
			get
			{
				return this._Text;
			}
			set
			{
				if ((this._Text != value))
				{
					this.OnTextChanging(value);
					this.SendPropertyChanging();
					this._Text = value;
					this.SendPropertyChanged("Text");
					this.OnTextChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValidFrom", DbType="DateTime NOT NULL")]
		public System.DateTime ValidFrom
		{
			get
			{
				return this._ValidFrom;
			}
			set
			{
				if ((this._ValidFrom != value))
				{
					this.OnValidFromChanging(value);
					this.SendPropertyChanging();
					this._ValidFrom = value;
					this.SendPropertyChanged("ValidFrom");
					this.OnValidFromChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValidTo", DbType="DateTime")]
		public System.Nullable<System.DateTime> ValidTo
		{
			get
			{
				return this._ValidTo;
			}
			set
			{
				if ((this._ValidTo != value))
				{
					this.OnValidToChanging(value);
					this.SendPropertyChanging();
					this._ValidTo = value;
					this.SendPropertyChanged("ValidTo");
					this.OnValidToChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Image", DbType="NVarChar(255)")]
		public string Image
		{
			get
			{
				return this._Image;
			}
			set
			{
				if ((this._Image != value))
				{
					this.OnImageChanging(value);
					this.SendPropertyChanging();
					this._Image = value;
					this.SendPropertyChanged("Image");
					this.OnImageChanged();
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
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.CMS_NewsGroup")]
	public partial class NewsGroup : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _ID;
		
		private System.DateTime _Created;
		
		private System.DateTime _Modified;
		
		private string _Name;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(System.Guid value);
    partial void OnIDChanged();
    partial void OnCreatedChanging(System.DateTime value);
    partial void OnCreatedChanged();
    partial void OnModifiedChanging(System.DateTime value);
    partial void OnModifiedChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    #endregion
		
		public NewsGroup()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Created", DbType="DateTime NOT NULL")]
		public System.DateTime Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				if ((this._Created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Modified", DbType="DateTime NOT NULL")]
		public System.DateTime Modified
		{
			get
			{
				return this._Modified;
			}
			set
			{
				if ((this._Modified != value))
				{
					this.OnModifiedChanging(value);
					this.SendPropertyChanging();
					this._Modified = value;
					this.SendPropertyChanged("Modified");
					this.OnModifiedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
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
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.CMS_NewsInNewsGroup")]
	public partial class NewsInNewsGroup : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _ID;
		
		private System.DateTime _Created;
		
		private System.DateTime _Modified;
		
		private System.Guid _NewsID;
		
		private System.Guid _NewsGroupID;
		
		private EntityRef<News> _News;
		
		private EntityRef<NewsGroup> _NewsGroup;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(System.Guid value);
    partial void OnIDChanged();
    partial void OnCreatedChanging(System.DateTime value);
    partial void OnCreatedChanged();
    partial void OnModifiedChanging(System.DateTime value);
    partial void OnModifiedChanged();
    partial void OnNewsIDChanging(System.Guid value);
    partial void OnNewsIDChanged();
    partial void OnNewsGroupIDChanging(System.Guid value);
    partial void OnNewsGroupIDChanged();
    #endregion
		
		public NewsInNewsGroup()
		{
			this._News = default(EntityRef<News>);
			this._NewsGroup = default(EntityRef<NewsGroup>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Created", DbType="DateTime NOT NULL")]
		public System.DateTime Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				if ((this._Created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Modified", DbType="DateTime NOT NULL")]
		public System.DateTime Modified
		{
			get
			{
				return this._Modified;
			}
			set
			{
				if ((this._Modified != value))
				{
					this.OnModifiedChanging(value);
					this.SendPropertyChanging();
					this._Modified = value;
					this.SendPropertyChanged("Modified");
					this.OnModifiedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NewsID", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid NewsID
		{
			get
			{
				return this._NewsID;
			}
			set
			{
				if ((this._NewsID != value))
				{
					if (this._News.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnNewsIDChanging(value);
					this.SendPropertyChanging();
					this._NewsID = value;
					this.SendPropertyChanged("NewsID");
					this.OnNewsIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NewsGroupID", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid NewsGroupID
		{
			get
			{
				return this._NewsGroupID;
			}
			set
			{
				if ((this._NewsGroupID != value))
				{
					if (this._NewsGroup.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnNewsGroupIDChanging(value);
					this.SendPropertyChanging();
					this._NewsGroupID = value;
					this.SendPropertyChanged("NewsGroupID");
					this.OnNewsGroupIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="News_NewsInNewsGroup", Storage="_News", ThisKey="NewsID", OtherKey="ID", IsForeignKey=true, DeleteOnNull=true, DeleteRule="CASCADE")]
		public News News
		{
			get
			{
				return this._News.Entity;
			}
			set
			{
				if ((this._News.Entity != value))
				{
					this.SendPropertyChanging();
					this._News.Entity = value;
					this.SendPropertyChanged("News");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="NewsGroup_NewsInNewsGroup", Storage="_NewsGroup", ThisKey="NewsGroupID", OtherKey="ID", IsForeignKey=true, DeleteOnNull=true, DeleteRule="CASCADE")]
		public NewsGroup NewsGroup
		{
			get
			{
				return this._NewsGroup.Entity;
			}
			set
			{
				if ((this._NewsGroup.Entity != value))
				{
					this.SendPropertyChanging();
					this._NewsGroup.Entity = value;
					this.SendPropertyChanged("NewsGroup");
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
}
#pragma warning restore 1591