# ![Zuken Logo](https://ulm-dev.zuken.com/Team-Erlangen/ALL/raw/branch/trunk/Images/logo2.png) BorrowUtility

## Table of Contents

- [About BorrowUtility](#about-borrowutility)
- [Key Features](#key-features)
- [System Requirements](#system-requirements)
- [Installation and Usage](#installation-and-usage)
  - [As a Standalone Application](#as-a-standalone-application)
  - [As a Library](#as-a-library)
- [Command Line Parameters](#command-line-parameters)
- [Project Metadata](#project-metadata)
- [License](#license)

## About

- **BorrowUtility** is a .NET application and library specifically designed for managing the borrowing and returning of software licenses or other managed resources. 
- It provides a standardized, auditable workflow for these operations, including status tracking and expiration management.
- Primarily, this utility is intended for integration with the Zuken E3.HarnessAnalyzer licensing infrastructure but can be adapted for other scenarios requiring controlled resource borrowing. 
- It offers both a graphical user interface for interactive use and command-line options for automation and integration into broader business processes.

## Key Features

- **License Management:** Borrow and return licenses or resources through a user-friendly interface.
- **Status Tracking:** Monitor the current status and expiration dates of borrowed items.
- **Audit Logging:** Comprehensive logging of all borrow and return operations for traceability.
- **E3.HarnessAnalyzer Integration:** Designed to work seamlessly with the Zuken E3.HarnessAnalyzer licensing system.
- **Automation Support:** Command line switches facilitate scripting and automated workflows.
- **Multi-Targeted:** Supports .NET Framework 4.7.2, .NET 6, and .NET 8 environments.

## Workflow

[![](https://mermaid.ink/img/pako:eNqlGGtv2zbwrwgqOqyAk9n1M_qwIXGcNUiaFPHaYpP9QZZoW4hMGhRVJ7Xz33fkkZT8SAEz_iDe-3i8E-_ktR-zhPiBP83YKp5HXHi3DyPqwW8oAPs9VMv4g3dy8qf3JeI5OeezPFSQJ8ExSluWErzOL4o0S_4huVgraEj4D8L_ekFhfFaEpNLmX5JvvIeCSkIeAuApaKwsDp5S2Ix8jj8cVr9jG6BdkkkxW6un8aaJpQ-Fhkj8DOGP9-WkMYicJhFPQgOMq7tH2d-slNrl1-twOGcrWLdkv14r7nksUkbXuGyfxfv33gXjHFSvIBFIQzm1HeRtvP6cxI94lqGCPUS0twpbOUTw_matafc3xqvh2GAH4ICH6jk-IKLO7X4pN5RjiBrRwhrTXjMSixAX7zaNCc2JNaqIUuyy4JFUAkFhES1mUBRkGH74hbOY5Lk-KSOquUoUQRlwEUtRE66h23Bv2ewqSrMQVk8Cushk-Ac0VPTD6Ae5plMWSsCTkIlJM5QJMHh_o8zqLWghRccaWSaRICEuu5UCdfBARMHp4TpA3sb7mwh9rnkIsDlk46zCVi4_RTnga1ispDkZZNlzuWOAhXds12JFDF9TtRHIeqi3u10Olm0KQlp9rSC0Ycgkqtk8I2rzrD1JUW1_L8-GbuNBwq9TvaWko1swnWwEq-kumZWtvJb2CreS_J2Mf0vJSt4josgPpx15Gy2D7x_CY3tXS2W8KJcpf0YZhHcLTF6ih_1IDlwyjE5Tvljr1ZytRu3Rot4-T52guoleqznDLMXP80ede4C2825ZKrrvERWIrjX1PMuM3ZJZuesZCISlrK0mgJXFq5RGWfqThAYYHzQmIzYSB6LYZxvMdq9qFlT1VXoCUtXLt0fVF8UuGXO7R7ZZHornLKUzb5WKuTfJIugVE8YTwnMPGpYHLesROkLMMgaUKexmQoRQFCp4BD1VKa7mKfgW5ElvPwerBEcDb5pmWfCuPWi2Lxq1XHD2SIJ3dfXT6MkqTcQ8-Lh8qmqr-jte2UZmrgcKrRtq3oQyyQpSdVNOJOir0e60-_Vf-mpsb9TMIs4GcEZw1baThasBGDtcVauThKsNMxM4x4_9wvnwzQzhbMBMFs4BmMHA1QD2Def048Xhql4dIpzfIDsHvKEI5KXonkN9h7tHYLv92w7hbbnUDd5VXTcLV_WyB7vnQfZbV23bS483YLvGJYnTXF4HO22D8YjOyB8TeNFp1Wf18xLdDjrtRv2ofZsvSld9PZq5qtsvOFcD9iPI1YD-bHBVt5O5qwEzlzrrmxnP1UBlijzeRDmxq3Fxp3A5SbZecSWDTi66jX6jf2Snkd9Gzvrld5azCZx-j9H2a_6Mp4kfTKMsJzV_Qfgikri_loZHvpiTBRn5AYDy0Eb-iL6A0jKi_zG28APBC1DjrJjNrZFCdc3LNJrxqBQhFCbnPiuo8IOPbWXCD9b-kx_0GqedXrfd7PSavXqnV2_W_GeQabVOO0A9q3fPmt1W86Xm_1Qu66e9busMfr0GPECzZVwOklQwbvcRFYINn2lstkAU-zP-W6f-tHv5H1D9Xlk?type=png "Click to view interactive diagram")](https://newmo-oss.github.io/mermaid-viewer/?sequence-number=0#eJylV19vGzcMf9+nEFCkaIEFjTu4A/KwwXGS1ogbF/GyoRDu4Xwn2werJ0PS9ZLZ++6DSEon/0kBq37wUSR/pEjqRN5cqrZY5tqy8cMvjDE2tbm2bzg8srfs/PwP9iXXRgz0wnCgmCMzUA4S0BuZq6aS5V/C2A1QU6G/C/3nf6ALf5GKg2y/CrNlD03tGIY/NDUDKgN7N0+VfcPdf/b2KPpebdnIXItZs9jAP7kiXucBlhyZn1UpsgM1Z2pq87rMdck9kXUbR83XQQc2+Dji06Vq2eMo0nwcgWxQ2ErVG3zEKTg7Y1dKa9WyW6laYKES7ANFWzZcimKF+eNAM1ygo0gKzpCc3G2IN7kjj14QYrzRWmkO/9mhBiRrsna7MRgaLVCXFuRSisJyfLBxVYjaCG8SeE7rutG5w/CpsGGBWn6FegoD51+0KoQxlCLSJCFoIulCbQqnSYF6dgh0rBa3eSX5WC2YI+hIucAPARD3NP8uRvVccUcwR1E4xAcDY7WY3IFR8o86wMYzsS5zKzg+dk/G2Rl7ELbR9dHKo2jLPgpL2TT8owipJUeRFNx9ys24KjafchMUKSMoCfm4V+Oq4Pdqz16khe8i7GKytpy2unMAgtQfAWfzhSNAZtm1QlQoLS59acmN0yTr+6X17BAKMn5Y3R0MBfZNUX2RjCrcyaJ9vFDpSBjVe6fIf1eidReFbczRSqNoSyr4niGd+SvYQfEKXFf6GVWQ3j1P7nI86sMJtmyo6nmlv23oSQmlVcgnwg5EkDa4aV44YV7WaQ/Mioo9MKudQgcJhPVPXltcbog7kJKsdrLo/lYDKXmn6g/PQEqwd1vVuaz+FdwT2TFTLlavcBjAgdQvQi/qMg8HLbrmgQlv2D6T7oE9LpZyn+uLOrXPsqoXrK3sks1kXqzYTOlSaMPyumRlrldCs0JJpQ2bK81mwlrg1FbnxiKwXVZWMCuecN/GPkuB3Z3NKykvX/Vvfutf9X41VquVuHx1AT9anrdVaZeX79dPERgO28lYH5V//WtVurvLhzGTjYh8dBMFOur1P/SHFz901NvZpB8mUvHY6BPBYTpIxD+OUpHxRJBownf31NixDaSm3U8DqXg/I6Tu3jf5RDw2hNSy4yWRiI5HgtSXJvT19OK7yy+5eHRLJ28/9O+fSsBPFZG6diKaOkIiuuuuyRVwrTQRHBrlyXjfG65FURn39u81B6XzeiHezbRq68hh/B2IPm8+9HsXp+zZf/slwmnSSkSHb65EfPh2ScTT0J+IDrN1It7PmKlwP7Ql4qOh8GQLYeyG+W/vsGpRxi80qKCHq997w97wtG7ivmpS4d0HUqoFnGVPAf8PHoC8Yg==)

## Installation and Usage

### As a Standalone Application

1.  Deploy the `BorrowUtility.exe` and its dependencies to a desired location on the user's machine.
2.  Launch `BorrowUtility.exe` to access the graphical interface for managing licenses.
3.  For automated tasks, `BorrowUtility.exe` can be invoked with [command line parameters](#command-line-parameters).

### As a Library

1.  Include the `BorrowUtility` project in your .NET solution within Visual Studio.
2.  Alternatively, add a reference to the compiled `BorrowUtility.dll` in your existing project.
3.  Leverage the public API provided by the library to integrate license borrowing functionalities directly into your application logic.

## Command Line Parameters

The `BorrowUtility.exe` supports the following command line switches for specialized operations:

| Switch            | Description                                                                    |
|-------------------|--------------------------------------------------------------------------------|
| `/NoSwitch`       | Default mode. Launches the application with the standard graphical user interface. |
| `/BuildServerTest`| Executes the application in a build server test mode and then exits. Useful for CI checks. |
| `/Debug`          | Enables debug mode. This may activate additional UI elements or logging for troubleshooting. |

**Usage Example:**
To run the utility in debug mode:

```
BorrowUtility.exe /Debug
```

Multiple switches can be combined if their functionalities are compatible (e.g., `BorrowUtility.exe /Debug /BuildServerTest`).

## Project Metadata

| Field               | Value                                            |
|---------------------|--------------------------------------------------|
| Project Name        | BorrowUtility                                    |
| Type                | Utility Application & .NET Library               |
| Supported Platforms | .NET Framework 4.7.2, .NET 6, .NET 8             |
| Primary Author      | Zuken                                            |
| Company             | Zuken E3 GmbH                                    |
| Primary Integration | Zuken E3.HarnessAnalyzer Licensing System        |

## License

This project is licensed under the Zuken License.  
Use of this application is subject to the terms and conditions described in the [Zuken E3 GmbH License Agreement](https://zuken.blob.core.windows.net/licenses/LICENSE).