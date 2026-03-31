using Core.Configs.Providers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Configs
{
    public class DataSource : ScriptableObject
    {
        [Title("Источник данных", titleAlignment: TitleAlignments.Centered)]
        
        [Space(10)]
        [Required("Укажи активный провайдер данных!")]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public GameConfigProvider ActiveProvider;
    }
}