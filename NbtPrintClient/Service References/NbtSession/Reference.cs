﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CswPrintClient1.NbtSession {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CswWebSvcSessionAuthenticateData.Authentication.Request", Namespace="http://schemas.datacontract.org/2004/07/ChemSW.WebSvc")]
    [System.SerializableAttribute()]
    public partial class CswWebSvcSessionAuthenticateDataAuthenticationRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string CustomerIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsMobileField;
        
        private string PasswordField;
        
        private string UserNameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string CustomerId {
            get {
                return this.CustomerIdField;
            }
            set {
                if ((object.ReferenceEquals(this.CustomerIdField, value) != true)) {
                    this.CustomerIdField = value;
                    this.RaisePropertyChanged("CustomerId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public bool IsMobile {
            get {
                return this.IsMobileField;
            }
            set {
                if ((this.IsMobileField.Equals(value) != true)) {
                    this.IsMobileField = value;
                    this.RaisePropertyChanged("IsMobile");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Password {
            get {
                return this.PasswordField;
            }
            set {
                if ((object.ReferenceEquals(this.PasswordField, value) != true)) {
                    this.PasswordField = value;
                    this.RaisePropertyChanged("Password");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string UserName {
            get {
                return this.UserNameField;
            }
            set {
                if ((object.ReferenceEquals(this.UserNameField, value) != true)) {
                    this.UserNameField = value;
                    this.RaisePropertyChanged("UserName");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CswWebSvcReturn", Namespace="http://schemas.datacontract.org/2004/07/NbtWebApp.WebSvc.Returns")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CswPrintClient1.NbtSession.CswNbtWebServiceSessionCswNbtSessionReturn))]
    public partial class CswWebSvcReturn : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CswPrintClient1.NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationResponse AuthenticationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CswPrintClient1.NbtSession.CswWebSvcReturnBaseLogging LoggingField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CswPrintClient1.NbtSession.CswWebSvcReturnBasePerformance PerformanceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CswPrintClient1.NbtSession.CswWebSvcReturnBaseStatus StatusField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CswPrintClient1.NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationResponse Authentication {
            get {
                return this.AuthenticationField;
            }
            set {
                if ((object.ReferenceEquals(this.AuthenticationField, value) != true)) {
                    this.AuthenticationField = value;
                    this.RaisePropertyChanged("Authentication");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CswPrintClient1.NbtSession.CswWebSvcReturnBaseLogging Logging {
            get {
                return this.LoggingField;
            }
            set {
                if ((object.ReferenceEquals(this.LoggingField, value) != true)) {
                    this.LoggingField = value;
                    this.RaisePropertyChanged("Logging");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CswPrintClient1.NbtSession.CswWebSvcReturnBasePerformance Performance {
            get {
                return this.PerformanceField;
            }
            set {
                if ((object.ReferenceEquals(this.PerformanceField, value) != true)) {
                    this.PerformanceField = value;
                    this.RaisePropertyChanged("Performance");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CswPrintClient1.NbtSession.CswWebSvcReturnBaseStatus Status {
            get {
                return this.StatusField;
            }
            set {
                if ((object.ReferenceEquals(this.StatusField, value) != true)) {
                    this.StatusField = value;
                    this.RaisePropertyChanged("Status");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CswWebSvcSessionAuthenticateData.Authentication.Response", Namespace="http://schemas.datacontract.org/2004/07/ChemSW.WebSvc")]
    [System.SerializableAttribute()]
    public partial class CswWebSvcSessionAuthenticateDataAuthenticationResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AuthenticationStatusField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CswPrintClient1.NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationResponse.Expired ExpirationResetField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TimeOutField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string AuthenticationStatus {
            get {
                return this.AuthenticationStatusField;
            }
            set {
                if ((object.ReferenceEquals(this.AuthenticationStatusField, value) != true)) {
                    this.AuthenticationStatusField = value;
                    this.RaisePropertyChanged("AuthenticationStatus");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CswPrintClient1.NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationResponse.Expired ExpirationReset {
            get {
                return this.ExpirationResetField;
            }
            set {
                if ((object.ReferenceEquals(this.ExpirationResetField, value) != true)) {
                    this.ExpirationResetField = value;
                    this.RaisePropertyChanged("ExpirationReset");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TimeOut {
            get {
                return this.TimeOutField;
            }
            set {
                if ((object.ReferenceEquals(this.TimeOutField, value) != true)) {
                    this.TimeOutField = value;
                    this.RaisePropertyChanged("TimeOut");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
        [System.Runtime.Serialization.DataContractAttribute(Name="CswWebSvcSessionAuthenticateData.Authentication.Response.Expired", Namespace="http://schemas.datacontract.org/2004/07/ChemSW.WebSvc")]
        [System.SerializableAttribute()]
        public partial class Expired : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
            
            [System.NonSerializedAttribute()]
            private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private string NewPasswordField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private string PasswordIdField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private string UserIdField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private string UserKeyField;
            
            public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
                get {
                    return this.extensionDataField;
                }
                set {
                    this.extensionDataField = value;
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public string NewPassword {
                get {
                    return this.NewPasswordField;
                }
                set {
                    if ((object.ReferenceEquals(this.NewPasswordField, value) != true)) {
                        this.NewPasswordField = value;
                        this.RaisePropertyChanged("NewPassword");
                    }
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public string PasswordId {
                get {
                    return this.PasswordIdField;
                }
                set {
                    if ((object.ReferenceEquals(this.PasswordIdField, value) != true)) {
                        this.PasswordIdField = value;
                        this.RaisePropertyChanged("PasswordId");
                    }
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public string UserId {
                get {
                    return this.UserIdField;
                }
                set {
                    if ((object.ReferenceEquals(this.UserIdField, value) != true)) {
                        this.UserIdField = value;
                        this.RaisePropertyChanged("UserId");
                    }
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public string UserKey {
                get {
                    return this.UserKeyField;
                }
                set {
                    if ((object.ReferenceEquals(this.UserKeyField, value) != true)) {
                        this.UserKeyField = value;
                        this.RaisePropertyChanged("UserKey");
                    }
                }
            }
            
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
            
            protected void RaisePropertyChanged(string propertyName) {
                System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
                if ((propertyChanged != null)) {
                    propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CswWebSvcReturnBase.Logging", Namespace="http://schemas.datacontract.org/2004/07/ChemSW.WebSvc")]
    [System.SerializableAttribute()]
    public partial class CswWebSvcReturnBaseLogging : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CustomerIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LogLevelField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LogglyInputField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ServerField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CustomerId {
            get {
                return this.CustomerIdField;
            }
            set {
                if ((object.ReferenceEquals(this.CustomerIdField, value) != true)) {
                    this.CustomerIdField = value;
                    this.RaisePropertyChanged("CustomerId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LogLevel {
            get {
                return this.LogLevelField;
            }
            set {
                if ((object.ReferenceEquals(this.LogLevelField, value) != true)) {
                    this.LogLevelField = value;
                    this.RaisePropertyChanged("LogLevel");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LogglyInput {
            get {
                return this.LogglyInputField;
            }
            set {
                if ((object.ReferenceEquals(this.LogglyInputField, value) != true)) {
                    this.LogglyInputField = value;
                    this.RaisePropertyChanged("LogglyInput");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Server {
            get {
                return this.ServerField;
            }
            set {
                if ((object.ReferenceEquals(this.ServerField, value) != true)) {
                    this.ServerField = value;
                    this.RaisePropertyChanged("Server");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CswWebSvcReturnBase.Performance", Namespace="http://schemas.datacontract.org/2004/07/ChemSW.WebSvc")]
    [System.SerializableAttribute()]
    public partial class CswWebSvcReturnBasePerformance : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double DbCommitField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double DbDeinitField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double DbInitField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double DbQueryField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double ServerInitField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double ServerTotalField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double TreeLoaderSqlField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double DbCommit {
            get {
                return this.DbCommitField;
            }
            set {
                if ((this.DbCommitField.Equals(value) != true)) {
                    this.DbCommitField = value;
                    this.RaisePropertyChanged("DbCommit");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double DbDeinit {
            get {
                return this.DbDeinitField;
            }
            set {
                if ((this.DbDeinitField.Equals(value) != true)) {
                    this.DbDeinitField = value;
                    this.RaisePropertyChanged("DbDeinit");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double DbInit {
            get {
                return this.DbInitField;
            }
            set {
                if ((this.DbInitField.Equals(value) != true)) {
                    this.DbInitField = value;
                    this.RaisePropertyChanged("DbInit");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double DbQuery {
            get {
                return this.DbQueryField;
            }
            set {
                if ((this.DbQueryField.Equals(value) != true)) {
                    this.DbQueryField = value;
                    this.RaisePropertyChanged("DbQuery");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double ServerInit {
            get {
                return this.ServerInitField;
            }
            set {
                if ((this.ServerInitField.Equals(value) != true)) {
                    this.ServerInitField = value;
                    this.RaisePropertyChanged("ServerInit");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double ServerTotal {
            get {
                return this.ServerTotalField;
            }
            set {
                if ((this.ServerTotalField.Equals(value) != true)) {
                    this.ServerTotalField = value;
                    this.RaisePropertyChanged("ServerTotal");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double TreeLoaderSql {
            get {
                return this.TreeLoaderSqlField;
            }
            set {
                if ((this.TreeLoaderSqlField.Equals(value) != true)) {
                    this.TreeLoaderSqlField = value;
                    this.RaisePropertyChanged("TreeLoaderSql");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CswWebSvcReturnBase.Status", Namespace="http://schemas.datacontract.org/2004/07/ChemSW.WebSvc")]
    [System.SerializableAttribute()]
    public partial class CswWebSvcReturnBaseStatus : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CswPrintClient1.NbtSession.CswWebSvcReturnBaseErrorMessage[] ErrorsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool SuccessField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CswPrintClient1.NbtSession.CswWebSvcReturnBaseErrorMessage[] Errors {
            get {
                return this.ErrorsField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorsField, value) != true)) {
                    this.ErrorsField = value;
                    this.RaisePropertyChanged("Errors");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Success {
            get {
                return this.SuccessField;
            }
            set {
                if ((this.SuccessField.Equals(value) != true)) {
                    this.SuccessField = value;
                    this.RaisePropertyChanged("Success");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CswNbtWebServiceSession.CswNbtSessionReturn", Namespace="http://schemas.datacontract.org/2004/07/ChemSW.Nbt.WebServices")]
    [System.SerializableAttribute()]
    public partial class CswNbtWebServiceSessionCswNbtSessionReturn : CswPrintClient1.NbtSession.CswWebSvcReturn {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CswPrintClient1.NbtSession.CswWebSvcReturnBaseData DataField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CswPrintClient1.NbtSession.CswWebSvcReturnBaseData Data {
            get {
                return this.DataField;
            }
            set {
                if ((object.ReferenceEquals(this.DataField, value) != true)) {
                    this.DataField = value;
                    this.RaisePropertyChanged("Data");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CswWebSvcReturnBase.ErrorMessage", Namespace="http://schemas.datacontract.org/2004/07/ChemSW.WebSvc")]
    [System.SerializableAttribute()]
    public partial class CswWebSvcReturnBaseErrorMessage : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DetailField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool ShowErrorField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TypeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Detail {
            get {
                return this.DetailField;
            }
            set {
                if ((object.ReferenceEquals(this.DetailField, value) != true)) {
                    this.DetailField = value;
                    this.RaisePropertyChanged("Detail");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ShowError {
            get {
                return this.ShowErrorField;
            }
            set {
                if ((this.ShowErrorField.Equals(value) != true)) {
                    this.ShowErrorField = value;
                    this.RaisePropertyChanged("ShowError");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Type {
            get {
                return this.TypeField;
            }
            set {
                if ((object.ReferenceEquals(this.TypeField, value) != true)) {
                    this.TypeField = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CswWebSvcReturnBase.Data", Namespace="http://schemas.datacontract.org/2004/07/ChemSW.WebSvc")]
    [System.SerializableAttribute()]
    public partial class CswWebSvcReturnBaseData : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool SucceededField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Succeeded {
            get {
                return this.SucceededField;
            }
            set {
                if ((this.SucceededField.Equals(value) != true)) {
                    this.SucceededField = value;
                    this.RaisePropertyChanged("Succeeded");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="NbtWebApp", ConfigurationName="NbtSession.Session")]
    public interface Session {
        
        [System.ServiceModel.OperationContractAttribute(Action="NbtWebApp/Session/Init", ReplyAction="NbtWebApp/Session/InitResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(System.ServiceModel.FaultException), Action="NbtWebApp/Session/InitFaultExceptionFault", Name="FaultException", Namespace="http://schemas.datacontract.org/2004/07/System.ServiceModel")]
        CswPrintClient1.NbtSession.CswWebSvcReturn Init(CswPrintClient1.NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationRequest Request);
        
        [System.ServiceModel.OperationContractAttribute(Action="NbtWebApp/Session/End", ReplyAction="NbtWebApp/Session/EndResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(System.ServiceModel.FaultException), Action="NbtWebApp/Session/EndFaultExceptionFault", Name="FaultException", Namespace="http://schemas.datacontract.org/2004/07/System.ServiceModel")]
        void End();
        
        [System.ServiceModel.OperationContractAttribute(Action="NbtWebApp/Session/ResetPassword", ReplyAction="NbtWebApp/Session/ResetPasswordResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(System.ServiceModel.FaultException), Action="NbtWebApp/Session/ResetPasswordFaultExceptionFault", Name="FaultException", Namespace="http://schemas.datacontract.org/2004/07/System.ServiceModel")]
        CswPrintClient1.NbtSession.CswNbtWebServiceSessionCswNbtSessionReturn ResetPassword(CswPrintClient1.NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationResponse.Expired Request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface SessionChannel : CswPrintClient1.NbtSession.Session, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SessionClient : System.ServiceModel.ClientBase<CswPrintClient1.NbtSession.Session>, CswPrintClient1.NbtSession.Session {
        
        public SessionClient() {
        }
        
        public SessionClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SessionClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SessionClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SessionClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public CswPrintClient1.NbtSession.CswWebSvcReturn Init(CswPrintClient1.NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationRequest Request) {
            return base.Channel.Init(Request);
        }
        
        public void End() {
            base.Channel.End();
        }
        
        public CswPrintClient1.NbtSession.CswNbtWebServiceSessionCswNbtSessionReturn ResetPassword(CswPrintClient1.NbtSession.CswWebSvcSessionAuthenticateDataAuthenticationResponse.Expired Request) {
            return base.Channel.ResetPassword(Request);
        }
    }
}
