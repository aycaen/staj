#pragma once   // birden fazla include yaz�m�n� �nler, sadece bir kez include eder.
#include <cstdint> //sabit uzunluklu t�rleri tan�mlar, �rne�in uint16_t, uint32_t gibi.
#ifdef BUILD_DLL  
#define DLL_API __declspec(dllexport) // dll in i�indeki fonksiyonlar� d��ar�ya a�ar, di�er programlar bu dll i kullanabilir.
#else
#define DLL_API __declspec(dllimport) // dll in i�indeki fonksiyonlar� i�eriye  a�ar, di�er programlar bu dll i kullanabilir.   
#endif
extern "C" {                                             // dll deki dosyan�n imzas�n� g�sterir.Her yerde extern "c" yazmak zorunda kalmay�z, include ile �a�r�l�r.
    DLL_API int ConvertBinToWavSimple(
        const char* binPath,
        const char* wavPath,
        uint16_t numChannels,
        uint32_t sampleRate,
        uint16_t bitsPerSample,
        uint32_t durationSeconds,
        bool isFromEnd);
}