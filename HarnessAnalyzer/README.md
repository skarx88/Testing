# ![Zuken Logo](https://ulm-dev.zuken.com/Team-Erlangen/ALL/raw/branch/trunk/Images/logo2.png) E3.HarnessAnalyzer

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [System Requirements](#system-requirements)
- [Installation](#installation)
- [Building Locally (CI-Build)](#building-locally-ci-build)
- [Licensing model (via FlexNet)](#licensing-model-via-flexnet)
  - [Registry Configuration](#registry-configuration)
  - [Customer-Specific Handling for Daimler](#customer-specific-handling-for-daimler)
- [Project Metadata](#project-metadata)
- [License](#license)

## Overview

- E3.HarnessAnalyzer is a sophisticated software solution designed for the comprehensive analysis and review of harness documents. 
- It facilitates efficient inspection, validation, and reporting of harness data. 
- The application supports multiple .NET runtimes (.NET Framework 4.7.2, .NET 6, and .NET 8) and utilizes FlexNet license management for feature control and compliance. 
- Specialized licensing and feature handling mechanisms are in place for specific customer requirements, such as those for Daimler.

> Notes: 
> - for a deeper dive into the application structure go to the [developer wiki](https://ulm-dev.zuken.com/Team-Erlangen/E3.App.Windows.HarnessAnalyzer/wiki/HarnessAnalyzer-developer-documentation).
> - for detailed usage instructions (user side), please refer to the [official documentation](https://ulm-dev.zuken.com/Team-Erlangen/E3.App.Windows.HarnessAnalyzer/raw/branch/trunk/Solution/Help/E3.HarnessAnalyzer.pdf).

## Key Features

-   **Comprehensive Analysis:** In-depth review and validation of harness documents.
-   **Modular Feature Set:** Controlled via FlexNet licensing and Windows Registry keys.
-   **Advanced Functionality:** Support for DSI, 3D visualization, PLM XML integration, schematic viewing, and topological comparison.
-   **Customer-Specific Adaptations:** Includes dynamic 3D license allocation for Daimler.
-   **Registry Integration:** Seamless feature management through the Windows Registry.
-   **Automated Build Support:** Includes capabilities for automated builds and Continuous Integration (CI).

## System Requirements

-   **Supported Operating Systems:** Windows 10, Windows 11 (64-bit).
-   **.NET Runtimes:**
    -   .NET Framework 4.7.2
    -   .NET 6
    -   .NET 8 (required for .NET 8-based components).
-   **Microsoft Visual C++ Redistributable:**
    -   For .NET Framework 4.7.2 components: Microsoft Visual C++ 2015–2019 Redistributable (x64).
    -   For .NET 6 and .NET 8 components: Microsoft Visual C++ 2015–2022 Redistributable (x64).
-   **FlexNet License Server:** Required for license validation (see [Licensing section](#licensing-via-flexnet)).

> **Note:** The specified C++ Redistributables are essential due to the use of native (C++/CLI) components. The latest supported versions can be downloaded from the [Microsoft website](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist).

## Installation

1.  Download the MSI Wix installer or the Bundle (Wix-Burn-.exe) installer from your designated distribution source.
2.  Execute the installer and follow the on-screen prompts to complete the installation process.
3.  The installer will configure the necessary registry keys for licensing.
4.  Ensure the FlexNet license server is accessible and configured as detailed in the [Licensing section](#licensing-via-flexnet).
5.  Verify that all prerequisite frameworks and redistributables listed under System Requirements are installed before launching the application.

For more detailed installation instructions, consult the [official documentation](https://ulm-dev.zuken.com/Team-Erlangen/E3.App.Windows.HarnessAnalyzer/raw/branch/trunk/Solution/Help/E3.HarnessAnalyzer.pdf).

## Building Locally (CI-Build)

1.  Checkout the repository to your local machine.
2.  Execute the `ci.bat` script (link: [ci.bat](https://ulm-dev.zuken.com/Team-Erlangen/E3.App.Windows.HarnessAnalyzer/src/branch/trunk/ci.bat)) to initiate a local build based on the `ci.yml` file's content.

For further information on the build process, refer to the [Build wiki](https://ulm-dev.zuken.com/Team-Erlangen/E3.App.Windows.HarnessAnalyzer/wiki/Build-Wiki).

## Licensing model (via FlexNet)

Application licensing is managed through [FlexNet license management](https://ulm-dev.zuken.com/Team-Erlangen/E3.Lib.Licensing.MFC/src/branch/trunk/Solution/Documentation/support_manual_flexnet.pdf) (see also [E3.Lib.Licensing](https://ulm-dev.zuken.com/Team-Erlangen/E3.Lib.Licensing)). The `E3HarnessAnalyzer` feature must be available in the license file or on the license server.

### Registry Configuration

Licensing behavior is controlled by Windows Registry keys. The primary key for E3.HarnessAnalyzer is created during installation.

-   If the registry entry `HKEY_CURRENT_USER\Software\Zuken\E3.HarnessAnalyzer` is not found, only the basic license is requested.
-   If this entry exists, all available sub-features from the server are requested automatically.

An additional subkey (DWORD32 type) named `AvailableFeatures` can be created under this registry path (typically by the IT department). This subkey should contain a bitmask combination of the following feature values, which will be requested at startup:

| Name                         | Value | Description                              |
| :--------------------------- | :---- | :--------------------------------------- |
| E3HarnessAnalyzer            | 1     | (Always required)                        |
| E3HarnessAnalyzerDSI         | 2     | DSI support                              |
| E3HarnessAnalyzer3D          | 4     | 3D visualization                         |
| E3HarnessAnalyzerPlmXml      | 8     | PLM XML integration                      |
| E3HarnessAnalyzerConBrow     | 16    | Schematics viewer                        |
| E3HarnessAnalyzerTopoCompare | 32    | Topological compare based on structure |

### Customer-Specific Handling for Daimler

A tailored licensing approach is implemented for Daimler environments, addressing their specific license distribution (high volume of basic licenses, lower count of 3D licenses).

The Daimler-specific handling operates as follows:

1.  A configuration switch `Check3DContent` (true/false) in the `app.config` (or `E3.HarnessAnalyzer.exe.config`) file enables this functionality. This switch is intended to be managed by Daimler during their deployment.
2.  The assembly's product name must be `E3.HarnessAnalyzer`, as registry key queries depend on this name.
3.  If `Check3DContent` is enabled, the application inspects the content of opened HCV (Harness Container Vehicle) files for 3D coordinates within the KBL (Kabelbaumliste) or the presence of JT files.
4.  When a 3D HCV file is opened (either from the command line or via MIME type/double-click), the `AvailableFeatures` registry value (which should initially reflect only the basic HA license) is dynamically updated to include the 3D license.
5.  The application then proceeds to open the file with 3D features enabled. This registry modification persists until a new file is opened, at which point the `AvailableFeatures` entry is re-evaluated and set accordingly based on the new file's content.
6.  Opening the Harness Analyzer application directly and subsequently opening an HCV file results in a different behavior: features are queried at startup based on the `AvailableFeatures` value present at that time.

## Project Metadata

| Field           | Value                                                                                                                                        |
| :-------------- | :------------------------------------------------------------------------------------------------------------------------------------------- |
| Product         | E3.HarnessAnalyzer                                                                                                                           |
| Authors         | Zuken Team Voyager                                                                                                                           |
| Company         | Zuken E3 GmbH                                                                                                                                |
| License Model   | FlexNet (see [Licensing model section](#licensing-model-via-flexnet))                                                                                    |
| Documentation   | [E3.HarnessAnalyzer.pdf](https://ulm-dev.zuken.com/Team-Erlangen/E3.App.Windows.HarnessAnalyzer/raw/branch/trunk/Solution/Help/E3.HarnessAnalyzer.pdf) |
| Build/CI        | [Build wiki](https://ulm-dev.zuken.com/Team-Erlangen/E3.App.Windows.HarnessAnalyzer/wiki/Build-Wiki)                                           |

## License

This software is proprietary and licensed by Zuken E3 GmbH.

-   Use of this application is subject to the terms and conditions outlined in the [Zuken E3 GmbH License Agreement](https://zuken.blob.core.windows.net/licenses/LICENSE).
-   Redistribution, modification, or commercial use is permitted only in accordance with the license terms.
-   For comprehensive details, please review the full license text or contact Zuken E3 GmbH.