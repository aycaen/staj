#include <windows.h>
#include <fstream>
#include <cstdint>
#include <vector>
#include "bin_to_wav.h"

#pragma pack(push, 1)
struct WAVHeader {
    char riff[4] = { 'R', 'I', 'F', 'F' };
    uint32_t chunkSize;
    char wave[4] = { 'W', 'A', 'V', 'E' };
    char fmt[4] = { 'f', 'm', 't', ' ' };
    uint32_t subchunk1Size = 16;
    uint16_t audioFormat = 1;
    uint16_t numChannels;
    uint32_t sampleRate;
    uint32_t byteRate;
    uint16_t blockAlign;
    uint16_t bitsPerSample;
    char data[4] = { 'd', 'a', 't', 'a' };
    uint32_t subchunk2Size;
};
#pragma pack(pop)

int ConvertBinToWavSimple(
    const char* binPath,
    const char* wavPath,
    uint16_t numChannels,
    uint32_t sampleRate,
    uint16_t bitsPerSample,
    uint32_t durationSeconds)
{
    uint32_t bytePerSample = bitsPerSample / 8;
    uint32_t byteRate = sampleRate * numChannels * bytePerSample;

    // binPath dosyası açılır, HANDLE Windows un dosya tanıtıcısıdır.
	HANDLE hFile = CreateFileA(binPath, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL); // dosyada sadece okuma,başka bir işlem aynı anda bu dposyayı okuyabilir,güvenlik öznitelikleri kullanılmaz,dosya varsa aç yoksa hata ver, standart dosya öznitelikleri kullanılır, yeni bir dosya oluştururken şablon dosya kullanılabilir kısmı null. 
	if (hFile == INVALID_HANDLE_VALUE) // dosya bulunmazsa veya açılamazsa INVALID_HANDLE_VALUE döner.
        return 1;

	LARGE_INTEGER fileSizeLI;  // 64 bitlik tam sayısal değer tutar, dosyanın boyutunu tutmak için kullanılır.
	if (!GetFileSizeEx(hFile, &fileSizeLI)) { // hFile ile belirtilen dosyanın boyutunu öğrenmeye çalışır, başarılı olursa, dosya boyutu fileSizeLI.QuadPart içinde saklanır.
        CloseHandle(hFile);
        return 2;
    }

    uint64_t totalSize = static_cast<uint64_t>(fileSizeLI.QuadPart); // dosyanın toplam boyutunu güvenli bir şekilde işaretsiz bir değişkene dönüştürerek saklar.
    uint64_t dataSize = (durationSeconds == 0) ? totalSize : static_cast<uint64_t>(durationSeconds) * byteRate;
    if (dataSize > totalSize) {
        CloseHandle(hFile);
        return 3;
    }

    // Memory-mapping oluşturma
	HANDLE hMap = CreateFileMappingA(hFile, NULL, PAGE_READONLY, 0, 0, NULL); // dosyayı okuma amaçlı haritalamak için, dosyanın içeriğini bellekte bir adres aralığına eşlemeye olanak tanır. güvenlik öznitelikleri null, bellek eşlemesi yalnızca okuma için, 0 0 ile dosyanın tamamı eşlenir, isimli bir eşleme oluşturulmaz.
    if (!hMap) {
        CloseHandle(hFile);
        return 4;
    }

    // Başlangıçtan dataSize kadar alanı haritala
	BYTE* fileData = (BYTE*)MapViewOfFile(hMap, FILE_MAP_READ, 0, 0, 0); // dosyanın içeriğini bellekteki bir adrese eşler, bu adresi fileData değişkenine atar. FILE_MAP_READ ile dosya okuma amaçlı haritalanır, 0 0 ile dosyanın başlangıcından itibaren eşleme yapılır, 0 ile dosyanın tamamı eşlenir.
    if (!fileData) {
        CloseHandle(hMap);
        CloseHandle(hFile);
        return 5;
    }

    // WAV header yaz
    WAVHeader header;
    header.numChannels = numChannels;
    header.sampleRate = sampleRate;
    header.bitsPerSample = bitsPerSample;
    header.byteRate = byteRate;
    header.blockAlign = numChannels * bytePerSample;
    header.subchunk2Size = static_cast<uint32_t>(dataSize);
    header.chunkSize = 36 + header.subchunk2Size;

    std::ofstream out(wavPath, std::ios::binary);
    if (!out) {
        UnmapViewOfFile(fileData);
        CloseHandle(hMap);
        CloseHandle(hFile);
        return 6;
    }

    out.write(reinterpret_cast<const char*>(&header), sizeof(WAVHeader));

    // Haritalanan veriden sadece gereken kısmı yaz
    out.write(reinterpret_cast<const char*>(fileData), static_cast<std::streamsize>(dataSize));

    out.close();

    // Belleği ve tanıtıcıları serbest bırak
    UnmapViewOfFile(fileData);
    CloseHandle(hMap);
    CloseHandle(hFile);

    return 0;
}
