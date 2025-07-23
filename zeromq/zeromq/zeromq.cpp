#include <zmq.hpp>
#include <nlohmann/json.hpp>
#include <iostream>
#include <fstream>
#include <filesystem>
#include <sstream>
#include "bin_to_wav.h"
using json = nlohmann::json;

int ConvertBinToWavSimple(
    const char* binPath,
    const char* wavPath,
    uint16_t numChannels,
    uint32_t sampleRate,
    uint16_t bitsPerSample,
    uint32_t durationSeconds);

int main() {
    zmq::context_t ctx(1);
    zmq::socket_t sock(ctx, zmq::socket_type::rep);
    sock.bind("tcp://*:5555");
    std::cout << "ZMQ Sunucu başlatıldı (port 5555)\n";

    while (true) {
        zmq::message_t req;
        sock.recv(req, zmq::recv_flags::none);

        std::string reqStr(static_cast<char*>(req.data()), req.size());
        nlohmann::json input;
        try {
            input = nlohmann::json::parse(reqStr);
        }
        catch (...) {
            sock.send(zmq::buffer("{\"status\":\"json_error\"}"), zmq::send_flags::none);
            continue;
        }

        std::string binPath = input["binPath"];
        std::string outputFolder = input["outputFolder"];
        uint16_t numChannels = input["numChannels"];
        uint32_t sampleRate = input["sampleRate"];
        uint16_t bitsPerSample = input["bitsPerSample"];
        uint32_t reqDuration = input.value("durationSec", 0u);  // 0 ise tamamı

        // Bin dosyasını açarak süresini belirle
        std::ifstream in(binPath, std::ios::binary | std::ios::ate);
        if (!in) {
            sock.send(zmq::buffer("{\"status\":\"file_error\"}"), zmq::send_flags::none);
            continue;
        }
        uint64_t totalSize = in.tellg();
        in.close();

        uint64_t bytesPerSecond = uint64_t(sampleRate) * numChannels * (bitsPerSample / 8);
        uint32_t totalSeconds = static_cast<uint32_t>(totalSize / bytesPerSecond);

        std::vector<std::string> outputs;

        uint32_t durSec = (reqDuration > 0) ? std::min(reqDuration, totalSeconds) : totalSeconds;
        std::ostringstream oss;

        if (reqDuration > 0) {
         
            oss << "ilk_" << (durSec / 3600) << "saat_" << ((durSec % 3600) / 60) << "dk.wav";
        }
        else {
           
            uint32_t hours = durSec / 3600;
            uint32_t mins = (durSec % 3600) / 60;
            oss << "tamami_" << hours << "saat";
            if (mins > 0) oss << "_" << mins << "dk";
            oss << ".wav";
        }

        std::string wavPath = outputFolder + "/" + oss.str();
        std::cout << "Donusturuluyor: " << wavPath << std::endl;

        if (ConvertBinToWavSimple(binPath.c_str(), wavPath.c_str(),
            numChannels, sampleRate, bitsPerSample,
            durSec) == 0)
        {
            outputs.push_back(wavPath);
        }

        nlohmann::json reply = {
            {"status", "success"},
            {"outputs", outputs}
        };
        sock.send(zmq::buffer(reply.dump()), zmq::send_flags::none);
    }

    return 0;
}
