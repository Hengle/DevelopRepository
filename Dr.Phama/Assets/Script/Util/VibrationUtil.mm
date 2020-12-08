#import <Foundation/Foundation.h>
#import <AudioToolBox/AudioToolBox.h>

extern "C" void _playSystemSoundWithDispose (int soundId)
{
    AudioServicesPlaySystemSound(soundId);
    AudioServicesDisposeSystemSoundID(soundId);
    
}

extern "C" void _playSystemSound (int soundId)
{
    AudioServicesPlaySystemSound(soundId);
    
}
