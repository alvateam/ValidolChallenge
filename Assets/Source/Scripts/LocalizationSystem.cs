using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Source.Scripts
{
    public class LocalizationSystem : MonoBehaviour
    {
    
        public async void SetLocale(int id)
        {
            await LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
        }
    }
}
