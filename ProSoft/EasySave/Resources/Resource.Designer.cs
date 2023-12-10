namespace EasySave.Resources {
    using System;
    using System.Runtime.CompilerServices;

    public class Resource {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        internal Resource() {
        }

        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("EasySave.Resources.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }

        public static string Confirm {
            get {
                return ResourceManager.GetString("Confirm", resourceCulture);
            }
        }

        public static string Confirm_Yes {
            get {
                return ResourceManager.GetString("Confirm.Yes", resourceCulture);
            }
        }

        public static string Confirm_No {
            get {
                return ResourceManager.GetString("Confirm.No", resourceCulture);
            }
        }

        public static string Language_fr_FR {
            get {
                return ResourceManager.GetString("Language.fr-FR", resourceCulture);
            }
        }

        public static string Language_en_US {
            get {
                return ResourceManager.GetString("Language.en-US", resourceCulture);
            }
        }

        public static string Path_Invalid {
            get {
                return ResourceManager.GetString("Path.Invalid", resourceCulture);
            }
        }

        public static string CreateSave_MaxSaves {
            get {
                return ResourceManager.GetString("CreateSave.MaxSaves", resourceCulture);
            }
        }

        public static string CreateSave_Name {
            get {
                return ResourceManager.GetString("CreateSave.Name", resourceCulture);
            }
        }

        public static string CreateSave_SourcePath {
            get {
                return ResourceManager.GetString("CreateSave.SourcePath", resourceCulture);
            }
        }

        public static string CreateSave_DestinationPath {
            get {
                return ResourceManager.GetString("CreateSave.DestinationPath", resourceCulture);
            }
        }

        public static string CreateSave_Type {
            get {
                return ResourceManager.GetString("CreateSave.Type", resourceCulture);
            }
        }

        public static string CreateSave_Type_Full {
            get {
                return ResourceManager.GetString("CreateSave.Type.Full", resourceCulture);
            }
        }

        public static string CreateSave_Type_Differential {
            get {
                return ResourceManager.GetString("CreateSave.Type.Differential", resourceCulture);
            }
        }

        public static string CreateSave_Success {
            get {
                return ResourceManager.GetString("CreateSave.Success", resourceCulture);
            }
        }

        public static string Forms_Back {
            get {
                return ResourceManager.GetString("Forms.Back", resourceCulture);
            }
        }

        public static string Forms_Exit {
            get {
                return ResourceManager.GetString("Forms.Exit", resourceCulture);
            }
        }

        public static string HomeMenu_Title {
            get {
                return ResourceManager.GetString("HomeMenu.Title", resourceCulture);
            }
        }

        public static string HomeMenu_Create {
            get {
                return ResourceManager.GetString("HomeMenu.Create", resourceCulture);
            }
        }

        public static string HomeMenu_Load {
            get {
                return ResourceManager.GetString("HomeMenu.Load", resourceCulture);
            }
        }

        public static string HomeMenu_Edit {
            get {
                return ResourceManager.GetString("HomeMenu.Edit", resourceCulture);
            }
        }

        public static string HomeMenu_Delete {
            get {
                return ResourceManager.GetString("HomeMenu.Delete", resourceCulture);
            }
        }

        public static string HomeMenu_ChangeLanguage {
            get {
                return ResourceManager.GetString("HomeMenu.ChangeLanguage", resourceCulture);
            }
        }

        public static string ChangeLanguage {
            get {
                return ResourceManager.GetString("ChangeLanguage", resourceCulture);
            }
        }



    }
}