﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.18051
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Arhivator {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Arhivator.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Отмена обработки..
        /// </summary>
        internal static string Cancel {
            get {
                return ResourceManager.GetString("Cancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на arhivator method inputfile outputfile
        ///method: 
        ///p - упаковка
        ///u - распаковка
        ///inputfile - входной файл
        ///outputfile - выходной файл
        ///.
        /// </summary>
        internal static string CommandInfo {
            get {
                return ResourceManager.GetString("CommandInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Обработка завершена..
        /// </summary>
        internal static string Compressor_Run_Finish {
            get {
                return ResourceManager.GetString("Compressor_Run_Finish", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Обработка файла....
        /// </summary>
        internal static string Compressor_Run_Start {
            get {
                return ResourceManager.GetString("Compressor_Run_Start", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Во время обработки возникли ошибки..
        /// </summary>
        internal static string Compressor_Run_WorckError {
            get {
                return ResourceManager.GetString("Compressor_Run_WorckError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Невозможно добавить в очередь еще одно задание.
        /// </summary>
        internal static string ErrorAddTask {
            get {
                return ResourceManager.GetString("ErrorAddTask", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка создания выходного файла.
        /// </summary>
        internal static string ErrorCreateOutputFile {
            get {
                return ResourceManager.GetString("ErrorCreateOutputFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Не верный формат файла.
        /// </summary>
        internal static string ErrorFileFormat {
            get {
                return ResourceManager.GetString("ErrorFileFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Некоректное имя входного файла или файл не существует.
        ///.
        /// </summary>
        internal static string ErrorInputFileName {
            get {
                return ResourceManager.GetString("ErrorInputFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка открытия входного файла.
        /// </summary>
        internal static string ErrorOpenInputFile {
            get {
                return ResourceManager.GetString("ErrorOpenInputFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Некоректное имя выходного файла
        ///.
        /// </summary>
        internal static string ErrorOutputFileName {
            get {
                return ResourceManager.GetString("ErrorOutputFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка в синтаксисе команды.
        ///.
        /// </summary>
        internal static string ErrorSyntax {
            get {
                return ResourceManager.GetString("ErrorSyntax", resourceCulture);
            }
        }
    }
}