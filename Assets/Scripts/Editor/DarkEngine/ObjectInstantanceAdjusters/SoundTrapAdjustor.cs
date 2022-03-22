using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Events.Devices;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    class SoundTrapAdjustor : IObjectInstanceAdjustor
    {
        AudioFileRepository audioRepo;
        UnitySS2AssetRepository assetRepository;

        public SoundTrapAdjustor(AudioFileRepository audioRepo, UnitySS2AssetRepository assetRepository)
        {
            this.audioRepo = audioRepo;
            this.assetRepository = assetRepository;
        }

        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            var soundName = darkObject.GetProp<ObjSoundNameProp>();
            if (soundName == null)
                return;

            var soundTrap = darkObject.gameObject.AddComponent<SoundTrap>();

            if (!audioRepo.DoesNameExist(soundName.Value))
            {
                Debug.LogError("Couldn't find sound with name: "+soundName.Value);
                return;
            }
            AudioClip clip = assetRepository.LoadAudioClip(audioRepo.GetPath(soundName.Value).relativePath);
            var audioSource = darkObject.gameObject.GetComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.playOnAwake = false;
        }
    }
}
