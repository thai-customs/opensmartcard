# OpenSmartcard

Linkage Center ActiveX Interface DLL for `sca_ope.dll`.

[Setup Development Environment on Visual Studio Code](https://docs.microsoft.com/en-us/dotnet/core/tutorials/library-with-visual-studio-code)

[Publish for getting dll dependency files](https://stackoverflow.com/questions/51604179/net-core-2-1-dotnet-exe-on-build-packages-are-missing)


## Building Project

```
# build for test
dotnet build
# build for production
dotnet publish
```

## Features

**Standard Functions**
- [ ] LicenseManager
- [ ] SetDebugOption
- [x] ListReader
- [x] OpenReader
- [ ] CloseReader
- [x] GetCardStatus
- [x] SelectApplet
- [x] GetCardInfo
- [x] GetInfoADM
- [ ] GetInfoACM
- [ ] GetInfoTemplate
- [x] ReadData
- [x] GetMatchStatus
- [ ] SwitchAuthorize
- [ ] DerivePIN
- [ ] EnvelopeGMS2
- [x] VerifyPIN
- [ ] ChangePIN
- [ ] OverwritePIN1
- [ ] VerifyPIN_OBJ
- [ ] ChangePIN_OBJ
- [ ] OverwritePIN1_OBJ
- [x] EnvelopeGMSx

**SASM Functions**
- [ ] GenerateMAC
- [ ] VerifyMAC
- [x] GetRMAC
- [ ] SetIgnoreCard

## Flow

**Officer Permission**

| Step | Lib | Test Case       | Input                                                   | Expected Result  |
| :--: | :-: | --------------- | ------------------------------------------------------- | ---------------- |
|    1 | OPE | listReader      | -                                                       | Card Reader List |
|    2 | OPE | openReader      | Card Reader Name                                        | -                |
|    3 | OPE | getCardStatus   | -                                                       | Card Type        |
|    4 | OPE | selectApplet    | MOI Applet                                              | -                |
|    5 | OPE | readData        | -                                                       | UID              |
|    6 | OPE | getCardInfo     | -                                                       | cardSN           |
|    7 | AMI | request9080     | UID, cardSN                                             | XKey32           |
|    8 | OPE | selectApplet    | ADM Applet                                              | -                |
|    9 | OPE | getInfoADM      | -                                                       | Laser Number     |
|   10 | OPE | verifyPIN       | -                                                       | PINCODE Dialog*  |
|   11 | OPE | getMatchStatus  | XKey32                                                  | Crypto           |
|   12 | OPE | envelopeGMSx    | SAS_INT_AUTH_FPKEY_ADMIN, Crypto                        | Envelope         |
|   13 | AMI | request9081     | UID, cardSN, XKey32, Envelope                           | Tkey32           |
|   14 | AMI | request5000     | Tkey32, Office Code, Service Version, Service Code, PID | JSON Data        |

*PINCODE Dialog is forked by SCAPI_OPE.dll . User have to input his PINCODE for verification.*

**Person Permission**

*In step 10, need `TKey32` from step 13 of Officer Permission*

| Step | Lib | Test Case       | Input                                                   | Expected Result  |
| :--: | :-: | --------------- | ------------------------------------------------------- | ---------------- |
|    1 | OPE | openReader      | Card Reader Name                                        | -                |
|    2 | OPE | getCardStatus   | -                                                       | Card Type        |
|    3 | OPE | selectApplet    | MOI Applet                                              | -                |
|    4 | OPE | readData        | -                                                       | UID              |
|    5 | OPE | getCardInfo     | -                                                       | cardSN           |
|    6 | OPE | selectApplet    | ADM Applet                                              | -                |
|    7 | OPE | getInfoADM      | -                                                       | Laser Number     |
|    8 | OPE | getRMAC         | -                                                       | Crypto           |
|    9 | OPE | envelopeGMSx    | SAS_INT_AUTH_FPKEY_ADMIN, Crypto                        | Envelope         |
|   10 | AMI | request5090     | PID, cardSN, Tkey32, Envelope                           | -                |
|   11 | AMI | request5000     | Tkey32, Office Code, Service Version, Service Code, PID | JSON Data        |

## Change Log
22/08/2023 - Support getRMAC function for *Person Permission* feature
