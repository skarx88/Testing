# E3.HarnessAnalyzer developer documentation

- The E3.HarnessAnalyzer application consists of several specialized modules that work together to provide a comprehensive harness analysis solution. 
- Each module has specific responsibilities and interfaces with other components and external libraries.

## Application modules

```
E3.HarnessAnalyzer
├── E3.HarnessAnalyzer.Project
│   ├── BaseProject
│   │   └── BaseHcvDocumentCollection
│   │       └── HcvDocument
│   │           ├── DocumentEntitiesCollection
│   │           ├── AsyncBundleRecalculator
│   │           └── TemporalEntitiesResult
│   ├── DiameterCalculation
│   └── WorkUnitWrappedWorkChunk
│       └── DocumentWorkUnitWrappedWorkChunk
│
├── E3.HarnessAnalyzer.Settings
│   ├── GeneralSettingsBase/GeneralSettings
│   ├── DiameterSettings
│   │   ├── CoreWireDiameter
│   │   ├── BundleDiameterRange
│   │   └── GenericDiameterFormulaParameters
│   ├── QMStampSpecifications
│   │   ├── ConnectorSpecification
│   │   ├── FixingSpecification
│   │   ├── ProtectionSpecification
│   │   └── SegmentSpecification
│   ├── WeightSettings
│   │   └── MaterialSpecification
│   ├── ViewPortSizeSettings
│   └── CarTransformationSettings
│
├── E3.HarnessAnalyzer.Shared
│   ├── Constants and Enumerations
│   ├── LicenseManager
│   ├── Collection Extensions
│   ├── XML Utilities
│   └── Path Utilities
│
├── E3.HarnessAnalyzer.Compatibility
│   ├── Printing Components
│   │   ├── Printer_2023
│   │   ├── PageEx_2023
│   │   ├── MilliInch_2023
│   │   ├── MilliInch96_2023
│   │   └── MilliInch100_2023
│   ├── Configuration Components
│   │   ├── ViewportSizeSetting_2023
│   │   ├── CavityCheckSettings_2023
│   │   └── CarTransformationSetting_2023
│   └── Data Model Components
│       └── CheckedCompareResult_2023
│
├── BorrowUtility
│   ├── UI Layer (BorrowUtility.vb)
│   ├── Business Logic Layer
│   │   ├── LicenseManager
│   │   └── FlmBorrowHandler
│   ├── Security Layer
│   │   └── SigningChecker
│   └── Integration Layer
│       ├── lmborrow.exe
│       ├── lmstat.exe
│       └── BorrowHelper.exe
│
└── External Dependencies
    ├── UI Components
    │   ├── E3.App.Windows.Components.AutoCompleteMenu
    │   ├── Infragistics.WinForms.*
    │   ├── VectorDraw.Drawing.Framework
    │   └── DevDept.Eyeshot.Control.Win
    ├── Core Libraries
    │   ├── E3.Lib.CoreTech.V2022
    │   └── E3.Lib.Licensing
    ├── Data Processing Libraries
    │   ├── E3.Lib.IO.Files.*
    │   ├── E3.Lib.Schema.*
    │   ├── E3.Lib.Locator.Splice
    │   ├── E3.Lib.Router.Topology
    │   ├── E3.Lib.Converter.*
    │   └── E3.Lib.DotNet.Expansions
    └── Interop Dependencies
        └── Zuken.E3.Interop
```

### Application Workflow (Sequence diagram)

Here's a sequence diagram showing a more detailled view of the main flow of the E3.HarnessAnalyzer application from startup to shutdown:

```mermaid
sequenceDiagram
    actor User
    participant App as Application
    participant MainForm
    participant MainState as MainStateMachine
    participant DocForm as DocumentForm
    participant TopoHub as TopologyHub
    participant SchView as SchematicsView
    participant D3D as D3DController

    User->>App: Start Application
    App->>MainForm: Create MainForm
    MainForm->>MainForm: Initialize()
    Note over MainForm: Load settings, initialize components
    MainForm->>MainState: New MainStateMachine(Me)
    MainForm->>TopoHub: InitializeTopologyHub()
    MainForm->>SchView: InitializeSchematicsView()
    MainForm->>D3D: New MainFormController(Me)
    App->>MainForm: Show UI
    MainForm->>MainForm: OpenDocumentFromCommandLine()

    User->>MainForm: Open Document
    MainForm->>MainState: OpenDocument()
    MainState->>DocForm: Create & Load Document
    DocForm->>MainForm: Set ActiveDocument
    MainState->>MainForm: DocumentOpenFinished
    MainForm->>SchView: AddDocumentToAdvConnectivityView()
    MainForm->>TopoHub: ShowTopologyHub()
    MainForm-->>User: Display Document

    User->>DocForm: Interact with Document
    DocForm->>MainForm: Send Messages/Events
    MainForm->>DocForm: Process Changes
    MainForm->>TopoHub: Update Topology View
    MainForm->>SchView: Update Schematic View

    User->>MainForm: Close Document
    MainForm->>MainState: CloseDocument()
    MainState->>DocForm: Close Document
    DocForm->>MainForm: Update UI
    MainForm->>SchView: RemoveDocumentFromConnectivityView()

    User->>MainForm: Close Application
    MainForm->>MainForm: OnClosing()
    MainForm->>MainForm: SaveSettings()
    MainForm->>App: Terminate Application
```

### Diagram explained (Application Workflow)

1.	The application follows a coordinated startup sequence:
    - Application bootstraps and creates the MainForm
    - MainForm initializes core components in a specific order
    - Key visualization subsystems (TopologyHub, SchematicsView, D3DController) are initialized
    - Any command-line documents are automatically opened

2.	The document management workflow demonstrates:
    - Clean separation of concerns with MainForm coordinating and MainStateMachine handling document operations
    - Multi-view synchronization where one document is represented in multiple specialized views
    - Event-based communication patterns to maintain view consistency

3.	The user interaction pattern shows:
    - Bidirectional communication between components
    - MainForm acting as a central mediator/coordinator for messages between subsystems
    - Consistent update propagation to ensure all views reflect the current document state

4.	The application shutdown process follows a controlled sequence:
    - Document closure being handled through MainStateMachine
    - View cleanup happening in reverse order of initialization
    - Settings persistence happening during shutdown

### Overview of Key Components

- **MainForm**: The main application window that hosts all components
- **MainStateMachine**: Manages application state and document operations
- **DocumentForm**: Represents an open document in the application
- **TopologyHub**: Manages the topology view of harnesses
- **SchematicsView**: Provides schematic views of harness connectivity
- **D3DController**: Manages 3D visualization components

The application follows a typical document-centric workflow where users can open multiple documents, interact with them through various views (2D, 3D, schematic), and perform operations like searching and comparing harness data.

### Mainform Dependency Diagram

- This diagram shows the key relationships between MainForm and other components of the E3.HarnessAnalyzer application. 
- The MainForm acts as the central coordinator, connecting user interface elements with the application's business logic and specialized visualization components.

```mermaid
classDiagram
    %% MAIN FORM AND PROJECT SECTION
    class MainForm {
        +Project: HarnessAnalyzerProject
        +ActiveDocument: DocumentForm
        +DiameterSettings: DiameterSettings
        +WeightSettings: WeightSettings
        +EntireRoutingPath: Dictionary~String, RoutingPath~
        +MainStateMachine: MainStateMachine
        +QMStampSpecifications: QMStampSpecifications
        +TopologyHub: TopologyHub
        +SchematicsView: ViewControl
    }

    %% PROJECT SECTION
    class HarnessAnalyzerProject {
        +Documents: BaseHcvDocumentCollection
        +DoWorkChunksAsync()
        +Invalidate()
        +Event ProjectStateChanging
        +Event ProjectStateChanged
    }

    class BaseHcvDocumentCollection {
        +Project: BaseProject
        +Add(document: HcvDocument)
        +Remove(document: HcvDocument)
        +Contains(document: HcvDocument): Boolean
        +GetEnumerator(): IEnumerator
    }

    class HcvDocument {
        +Entities: DocumentEntitiesCollection
        +Kbl: KblMapper
        +Model: EESystemModel
        +SchemaVersion: KblSchemaVersion
        +VariantUsedToActivate: Variant
        +RecalculateBundleDiameters()
        +SetModelObjectsActiveByKblIds()
        +Event EntitiesUpdated
    }

    class DocumentEntitiesCollection {
        +GetByKblIds(): List~Entity3D~
        +GetByEEObjectId(): Entity3D
        +Invalidate()
        +RegenAsync()
        +Event Updated
    }

    %% SETTINGS SECTION
    class IHarnessAnalyzerSettingsProvider {
        <<interface>>
        +DiameterSettings: DiameterSettings
        +GeneralSettings: GeneralSettingsBase
        +QMStampSpecifications: QMStampSpecifications
        +HasView3DFeature: Boolean
        +HasTopoCompareFeature: Boolean
        +HasSchematicsFeature: Boolean
    }

    class DiameterSettings {
        +GenericDiameterFormulaParameters: GenericDiameterFormulaParameters
        +CoreWireDiameters: List~CoreWireDiameter~
        +BundleDiameterRanges: List~BundleDiameterRange~
        +ResetToDefaults()
        +Save(filename: String)
        +LoadFromFile(filename: String): DiameterSettings
    }

    class CoreWireDiameter {
        +CrossSection: Double
        +OutsideDiameter: Double
        +IsCable: Boolean
        +Code: String
        +Color: String
    }

    class BundleDiameterRange {
        +From: Double
        +To: Double
        +Color: Color
        +Name: String
    }

    class GenericDiameterFormulaParameters {
        +IsolationThickness: Double
        +NumberOfCoresWiresModificator: Double
        +TolerancePercentage: Double
        +CalculateOutsideDiameter(cores: List~CoreWireDiameter~): Double
    }

    class WeightSettings {
        +MaterialSpecifications: List~MaterialSpecification~
        +SaveTo(filename: String)
        +LoadFromFile(filename: String): WeightSettings
    }

    class MaterialSpecification {
        +Id: String
        +Density: Double
        +Type: MaterialType
        +Name: String
        +CalculateWeight(volume: Double): Double
    }

    class QMStampSpecifications {
        +ConnectorSpecifications: List~ConnectorSpecification~
        +FixingSpecifications: List~FixingSpecification~
        +ProtectionSpecifications: List~ProtectionSpecification~
        +SegmentSpecifications: List~SegmentSpecification~
        +ResetToDefaults()
        +SaveTo(filename: String)
    }

    class ConnectorSpecification {
        +Type: String
        +ValidFunctions: List~String~
        +ValidVariants: List~String~
        +ValidateForType(connectorType: String): Boolean
    }

    class FixingSpecification {
        +Type: String
        +ValidFunctions: List~String~
        +ValidateForType(fixingType: String): Boolean
    }

    %% ROUTING PATH SECTION
    class RoutingPath {
        +Connections: Dictionary~String, RoutingConnection~
        +Harness: Harness
        +IsOriginHarness: Boolean
        +SignalName: String
        +AddPredecessor(routingConnection: RoutingConnection)
        +AddSuccessor(routingConnection: RoutingConnection)
        +GetAllConnections(): List~RoutingConnection~
    }

    class RoutingConnection {
        +FromConnector: Connector_occurrence
        +ToConnector: Connector_occurrence
        +CoreWires: List~Core_or_wire~
        +FromCavities: List~Cavity~
        +ToCavities: List~Cavity~
        +IsSuccessor: Boolean
        +IsValid(): Boolean
    }

    class Harness {
        +Id: String
        +PartNumber: String
        +Variants: List~Variant~
        +Connectors: List~Connector_occurrence~
        +Wires: List~Wire~
        +Segments: List~Segment~
        +GetConnector(connectorId: String): Connector_occurrence
        +GetSegment(segmentId: String): Segment
    }

    class Connector_occurrence {
        +Id: String
        +PartNumber: String
        +Type: String
        +Cavities: List~Cavity~
        +Position: Point3D
        +GetCavity(cavityId: String): Cavity
    }

    class Cavity {
        +Id: String
        +Number: String
        +Function: String
        +AssignedCoreWires: List~Core_or_wire~
        +ConnectorId: String
        +IsValid(): Boolean
    }

    %% TOPOLOGY SECTION
    class TopologyHub {
        +SelectedCompartments: List~CompartmentInfo~
        +ActiveCompartments: List~CompartmentInfo~
        +Initialize(topologyView: TopologyView)
        +SelectCompartments(harnessParts: List~String~)
        +SelectActiveCompartments()
        +ToggleActiveHarnesses()
        +Event CompartmentClicked
    }

    class CompartmentInfo {
        +Id: String
        +Name: String
        +Description: String
        +Position: Point3D
        +ContainedHarnesses: List~String~
        +ContainsHarness(harnessId: String): Boolean
    }

    class CarTopologyControl {
        +SelectedCompartments: List~CompartmentInfo~
        +ActiveCompartments: List~CompartmentInfo~
        +SetSelectedCompartments(compartments: List~CompartmentInfo~)
        +SetActiveCompartments(compartments: List~CompartmentInfo~)
        +Event CompartmentClicked
    }

    %% RELATIONSHIPS
    MainForm --|> IHarnessAnalyzerSettingsProvider : Implements
    MainForm *-- HarnessAnalyzerProject : Contains
    MainForm *-- DiameterSettings : Contains
    MainForm *-- WeightSettings : Contains
    MainForm *-- QMStampSpecifications : Contains
    MainForm *-- TopologyHub : Contains
    MainForm *-- RoutingPath : Manages

    HarnessAnalyzerProject *-- BaseHcvDocumentCollection : Contains
    BaseHcvDocumentCollection *-- HcvDocument : Contains
    HcvDocument *-- DocumentEntitiesCollection : Contains

    DiameterSettings *-- CoreWireDiameter : Contains
    DiameterSettings *-- BundleDiameterRange : Contains
    DiameterSettings *-- GenericDiameterFormulaParameters : Contains
    
    WeightSettings *-- MaterialSpecification : Contains
    
    QMStampSpecifications *-- ConnectorSpecification : Contains
    QMStampSpecifications *-- FixingSpecification : Contains

    RoutingPath *-- RoutingConnection : Contains
    RoutingPath *-- Harness : References

    RoutingConnection *-- Connector_occurrence : References
    RoutingConnection *-- Cavity : References

    Harness *-- Connector_occurrence : Contains
    Connector_occurrence *-- Cavity : Contains

    TopologyHub *-- CompartmentInfo : Contains
    TopologyHub *-- CarTopologyControl : Uses
```

#### Additional (non key-) component explanations

1. **Project & Document Management**:
   - Contains HarnessAnalyzerProject to manage document collections
   - Maintains ActiveDocument reference for current user focus
   - Coordinates document operations through MainStateMachine

2. **UI Management**:
   - Manages panes through TabToolPanesCollection
   - Controls specialized views (TopologyHub, SchematicsView)
   - Implements Windows message filtering (IMessageFilter)

3. **Settings Provider**:
   - Implements IHarnessAnalyzerSettingsProvider
   - Manages application configuration through various settings objects
   - Controls feature availability based on licensing

4. **Visualization Components**:
   - Coordinates with D3DController for 3D visualization
   - Manages SchematicsView for connectivity visualization
   - Handles topology visualization through TopologyHub

5. **System Services**:
   - Clipboard management (Attach/DetachFromClipboard)
   - Message processing (ProcessDocumentMessage)
   - Command-line handling (OpenDocumentFromCommandLine)

### MainStateMachine Dependency Diagram

```mermaid
classDiagram
    class MainStateMachine {
        +OverallConnectivity
        +XHcvFile
        +EntireCarSettings
        +CurrentLoadingDocumentsCount
        +OpenDocument()
        +CloseDocument()
        +SaveDocument()
        +SaveXhcv()
        +ShowInliners()
        +CompareDocuments()
        +PrintDocument()
        +ShowGeneralSettings()
        +Event DocumentOpenFinished
        +Event DocumentFilterItemCheckedChanged
    }

    class MainForm {
        +ActiveDocument
        +DiameterSettings
        +GeneralSettings
        +QMStampSpecifications
        +MainStateMachine
        +SearchMachine
        +TopologyHub
        +SchematicsView
        +D3D
        +Project
        +DocumentViews
        +GetAllDocuments()
        +OnStartLoadingXhcv()
    }

    class DocumentForm {
        +ActiveDrawingCanvas
        +KBL
        +Document
        +MainForm
        +HarnessConnectivity
        +IsExtendedHCV
        +IsDirty
        +Inliners
        +SaveDocument()
        +EnableWaitCursor()
        +OnMessage()
        +File
    }

    class DocumentFilterControl {
        +Event ItemCheckedChanged
    }

    class XhcvFile {
        +FullName
        +IsOpening
        +EntireCarSettings
        +CarTransformation
        +Open()
        +Save()
        +Close()
    }

    class HcvFile {
        +FullName
        +KblDocument
        +Owner
    }

    class CompareForm {
        +Event CompareHubSelectionChanged
        +Event LogMessage
    }

    class GraphicalCompareForm {
        +Event FormClosed
    }

    class InlinersForm {
        +Event InlinerSelectionChanged
    }

    class UltraDockManager {
        +PaneFromKey()
        +Event PaneHidden
    }

    class UltraToolbarsManager {
        +Tools
        +Ribbon
        +BeginUpdate()
        +EndUpdate()
        +Event ToolClick
    }

    class UltraTabbedMdiManager {
        +TabGroups
        +TabFromForm()
        +Event TabClosed
        +Event TabClosing
        +Event TabSelected
    }

    class OverallConnectivity {
        +HarnessPartNumber
        +HarnessConnectivities
        +InlinerCavityPairs
    }

    class EntireCarSettings {
        +HasData
        +LoadFromContainerFile()
    }

    class VirtualInlinerPair {
        +Id
        +InlinerIdA
        +InlinerIdB
        +DocumentA
        +DocumentB
        +ConnectorOccsWithKblMapperA
        +ConnectorOccsWithKblMapperB
        +GetCavityOccurenceMapping()
    }

    MainStateMachine --> MainForm: References
    MainStateMachine --> XhcvFile: Manages
    MainStateMachine --> OverallConnectivity: Manages
    MainStateMachine --> EntireCarSettings: Manages
    MainStateMachine --> DocumentFilterControl: Uses
    MainStateMachine --> CompareForm: Controls
    MainStateMachine --> GraphicalCompareForm: Controls
    MainStateMachine --> InlinersForm: Controls

    MainStateMachine --> UltraDockManager: References
    MainStateMachine --> UltraToolbarsManager: References
    MainStateMachine --> UltraTabbedMdiManager: References

    MainStateMachine --> VirtualInlinerPair: Creates/Manages
    
    MainStateMachine ..> DocumentForm: Creates/Opens/Closes
    MainStateMachine ..> HcvFile: Opens
```

#### Key Interaction Patterns

##### Application State Management

- **MainStateMachine** serves as the central controller for the application's core operations
- It orchestrates document lifecycle (open, close, save) and handles high-level user interactions
- Maintains the application's global state and exposes key events for state changes

##### Document Management Flow

1. **Document Creation**:
   - `OpenDocument()` initializes document loading sequence
   - Creates appropriate file handlers (HcvFile, XhcvFile) based on file type
   - Constructs and initializes DocumentForm instances to display content

2. **Document Comparison**:
   - `CompareDocuments()` launches comparison workflows between multiple documents
   - Creates and manages CompareForm or GraphicalCompareForm based on comparison type
   - Handles selection synchronization between comparison views and documents

3. **Inliner Management**:
   - Discovers and manages virtual inliner pairs between harnesses
   - `ShowInliners()` presents interface for visualizing and managing inliner connections
   - Maintains cavity mappings and connector relationships

##### UI Infrastructure
- Integrates with Infragistics UI framework (UltraDockManager, UltraToolbarsManager, UltraTabbedMdiManager)
- Registers and handles UI events from ribbon, toolbars, and MDI tabs
- Routes commands from UI to appropriate handlers throughout the application

##### Multi-Document Architecture
- Handles the complexity of working with multiple documents simultaneously
- Maintains OverallConnectivity data structure for cross-document connectivity information
- Synchronizes selection, visibility, and data between documents when appropriate

##### Event Flow
- Exposes events like DocumentOpenFinished to allow subscribers to react to completed operations
- Listens for events from UI components and routes them to appropriate handlers
- Maintains thread safety for event handling through locks and thread marshaling

### DocumentForm Dependency Diagram

```mermaid
classDiagram
    class DocumentForm {
        +ActiveDrawingCanvas
        +KBL
        +Id
        +Document
        +MainForm
        +HarnessConnectivity
        +OnCanvasSelectionChanged()
        +OnHubSelectionChanged()
        +SelectRowsInGrids()
        +SelectIn3DView()
        +OpenDocument()
        +SaveDocument()
    }

    class MainForm {
        +ActiveDocument
        +DiameterSettings
        +GeneralSettings
        +QMStampSpecifications
        +EntireRoutingPath
        +MainStateMachine
        +SearchMachine
        +TopologyHub
        +SchematicsView
        +D3D
        +Project
        +DocumentViews
        +SelectSchematicsEntities()
    }

    class MainStateMachine {
        +OverallConnectivity
        +XHcvFile
        +EntireCarSettings
        +CurrentLoadingDocumentsCount
        +OpenDocument()
        +CloseDocument()
        +SaveDocument()
        +Event DocumentOpenFinished
    }

    class HarnessAnalyzerProject {
        +Documents
        +SynchronizationContext
        +ConnectionStrings
        +OpenFromPath()
        +SaveToPath()
        +AddDocumentFromPath()
    }

    class DrawingCanvas {
        +vdCanvas
        +GroupMapper
        +CanvasSelection
        +InformationHubSelectionChanged()
        +FilterActiveObjects()
        +DisplayDrawing()
        +Event CanvasSelectionChanged
    }

    class InformationHub {
        +ActiveGrid
        +utcInformationHub
        +CurrentRowFilterInfo
        +RowFiltes
        +CanvasSelectionChangedSyncRowsAction()
        +SelectRowsInGrids()
        +InitializeDataSources()
        +FilterActiveObjects()
        +Event HubSelectionChanged
    }

    class Document3DControl {
        +SelectEntitiesByKbl()
        +ZoomFitSelection()
        +Invalidate()
        +Event SelectionChanged
    }

    class TopologyHub {
        +SelectCompartments()
        +SelectActiveCompartments()
        +Initialize()
        +Event CompartmentClicked
    }

    class SchematicsView {
        +Entities
        +ActiveEntities
        +ContainsItem()
        +Update()
        +SelectEntities()
    }

    class HcvDocument {
        +IsOpen
        +Kbl
        +Model
        +Entities
        +Open()
        +Close()
        +SetView()
    }

    class XhcvFile {
        +OpenMode
        +FullName
        +IsOpening
        +CarTransformation
        +Save()
        +Close()
    }

    class HarnessConnectivity {
        +HarnessPartNumber
        +HarnessConnectivities
        +InlinerCavityPairs
    }

    %% Main application structure
    MainForm --> HarnessAnalyzerProject: Project
    MainForm --> MainStateMachine
    MainForm --> TopologyHub
    MainForm --> SchematicsView
    MainForm --> DocumentForm: ActiveDocument
    
    MainStateMachine --> XhcvFile
    MainStateMachine --> HarnessConnectivity: OverallConnectivity
    MainStateMachine --> MainForm
    MainStateMachine ..> DocumentForm: Creates/Opens/Closes

    HarnessAnalyzerProject --> HcvDocument: Documents
    
    %% Document structure
    DocumentForm --> MainForm
    DocumentForm --> HcvDocument: Document
    DocumentForm --> DrawingCanvas: _activeDrawingCanvas
    DocumentForm --> InformationHub: _informationHub
    DocumentForm --> Document3DControl: _D3DControl
    DocumentForm --> HarnessConnectivity
    
    %% Selection flow (bidirectional)
    DrawingCanvas ..> DocumentForm: "1. Triggers CanvasSelectionChanged"
    DocumentForm ..> InformationHub: "2. Calls CanvasSelectionChangedSyncRowsAction"
    
    InformationHub ..> DocumentForm: "1. Triggers HubSelectionChanged"
    DocumentForm ..> DrawingCanvas: "2. Calls InformationHubSelectionChanged"
    DocumentForm ..> Document3DControl: "3. Calls SelectEntitiesByKbl"
    DocumentForm ..> MainForm: "4. Calls SelectSchematicsEntities"
    MainForm ..> SchematicsView: "5. Updates Schematic selection"
    DocumentForm ..> TopologyHub: "6. Calls SelectCompartments"
```

#### Key Interaction Patterns

##### Document Structure

- **MainStateMachine** orchestrates document lifecycle (create, open, close)
- **HarnessAnalyzerProject** manages collection of documents
- **DocumentForm** serves as container for document-specific views and data

##### Selection Flow Explanation

###### From DrawingCanvas to InformationHub:

1. User selects elements in the DrawingCanvas
2. DrawingCanvas raises CanvasSelectionChanged event
3. DocumentForm.OnCanvasSelectionChanged captures this event
4. DocumentForm creates a CanvasSelection object containing KBL IDs
5. DocumentForm calls InformationHub.CanvasSelectionChangedSyncRowsAction
6. InformationHub selects the corresponding rows in its grids

###### From InformationHub to DrawingCanvas:

1. User selects rows in InformationHub grids
2. InformationHub raises HubSelectionChanged event with ObjectIds
3. DocumentForm.OnHubSelectionChanged captures this event
4. DocumentForm iterates through all DrawingCanvas tabs
5. For each visible DrawingCanvas, calls InformationHubSelectionChanged with the KBL IDs
6. DrawingCanvas highlights the corresponding entities

###### Extended Selection:
- When selection happens in either DrawingCanvas or InformationHub, it can also propagate to:
  - SchematicsView via MainForm.SelectSchematicsEntities
  - 3D view via DocumentForm.SelectIn3DView
  - TopologyHub via TopologyHub.SelectCompartments
  - CavitiesDocumentView via CavitiesDocumentView.Model.Selected.Reset

###### Helper Methods

- DocumentForm.SelectRowsInGrids: Public method to select rows in InformationHub grids
- DocumentForm.SelectIn3DView: Selects entities in 3D view based on KBL IDs
- DocumentForm.AddDimensionRelatedObjects: Enhances selection with related objects
- DocumentForm.TrySelectInDrawingCanvas: Helper to select in either 2D or 3D view

This bidirectional selection mechanism ensures that selections stay synchronized across all views of the harness data, providing a consistent user experience regardless of which view the user interacts with.

### TopologyHub Dependency Diagram

```mermaid
classDiagram
    class TopologyHub {
        +SelectedCompartments
        +ActiveCompartments
        +SelectCompartments()
        +SelectActiveCompartments()
        +Initialize()
        +InitializeInlinerConnections()
        +ToggleActiveHarnesses()
        +ShowTopologyHub()
        +LoadFilterSettings()
        +SaveFilterSettings()
        +Event CompartmentClicked
        +Event ModuleSelected
    }

    class MainForm {
        +ActiveDocument
        +GeneralSettings
        +MainStateMachine
        +SchematicsView
        +TopologyHub
        +D3D
    }

    class DocumentForm {
        +Id
        +Document
        +KBL
        +HarnessConnectivity
    }

    class InlinerIdentifier {
        +KBLPropertyName
        +IdentificationCriteria
        +PairIdentifiers
        +IsMatch()
        +GetPairRecognizer()
    }

    class CarTopologyControl {
        +SelectedCompartments
        +SetSelectedCompartments()
        +ActiveCompartments
        +SetActiveCompartments()
        +Event CompartmentClicked
        +Event ModuleSelected
    }

    class HarnessConnectivity {
        +HarnessPartNumber
        +HarnessConnectivities
        +InlinerCavityPairs
    }

    class OverallConnectivity {
        +HarnessPartNumber
        +HarnessConnectivities
        +InlinerCavityPairs
    }

    class UltraTabbedMdiManager {
        +TabGroups
        +ActiveTab
    }

    class InlinerConnection {
        +HarnessIdA
        +ConnectorIdA
        +HarnessIdB
        +ConnectorIdB
    }

    TopologyHub --> MainForm: References
    TopologyHub --> CarTopologyControl: Contains
    TopologyHub --> HarnessConnectivity: Uses
    TopologyHub --> OverallConnectivity: Uses
    TopologyHub --> UltraTabbedMdiManager: References
    
    TopologyHub ..> DocumentForm: Interacts with
    TopologyHub ..> InlinerIdentifier: Uses
    TopologyHub ..> InlinerConnection: Creates/Manages
```

#### Key Interaction Patterns

##### Vehicle Topology Representation
- **TopologyHub** provides a high-level view of vehicle compartments and harness installations
- Visualizes the physical location of harnesses within the vehicle structure
- Serves as a navigation aid for understanding harness placement and relationships

##### Selection Synchronization
1. **Bidirectional Selection Flow**:
   - When user selects compartments in topology view, relevant harnesses are highlighted in other views
   - When harnesses are selected in document views, corresponding compartments are highlighted in topology
   - `SelectCompartments()` is called by DocumentForm when selection changes in other views

2. **Active Document Filtering**:
   - `SelectActiveCompartments()` highlights only compartments relevant to active document
   - This helps users focus on the currently active harness while maintaining context

##### Inliner Visualization
- Discovers and visualizes inter-harness connections (inliners) that connect different harnesses
- `InitializeInlinerConnections()` analyzes all open documents to find potential inliner pairs
- Uses InlinerIdentifier to detect components that might function as inliners based on pattern matching

##### Filter Management
- Provides filtering capabilities to show/hide specific compartments and harnesses
- `LoadFilterSettings()`/`SaveFilterSettings()` persist filter preferences between sessions
- Filter state is maintained as part of user preferences

##### Vehicle Structure
- Works with a hierarchical model of vehicle compartments (body regions, modules)
- Allows drill-down from major compartments to specific installation locations
- `ToggleActiveHarnesses()` provides a quick way to focus on or expand from currently active harnesses

##### Event Propagation
- Exposes events like CompartmentClicked and ModuleSelected for other components to react to topology interactions
- Routes selection events from the CarTopologyControl to interested subscribers
- Maintains correct selection state across multiple documents and views

### SchematicsView Dependency Diagram

```mermaid
classDiagram
    class SchematicsView {
        +Entities
        +ActiveEntities
        +AutoZoomSelection
        +ContainsItem()
        +Update()
        +SelectEntities()
        +AddDocumentToAdvConnectivityView()
        +RemoveDocumentFromConnectivityView()
        +ExportToPDF()
    }

    class MainForm {
        +ActiveDocument
        +GeneralSettings
        +TopologyHub
        +SchematicsView
        +D3D
    }

    class DocumentForm {
        +Id
        +Document
        +KBL
        +HarnessConnectivity
    }

    class AdvConnectivityView {
        +SelectEntities()
        +AddDocument()
        +RemoveDocument()
        +ExportToPDF()
        +ContainsItem()
        +Event SelectionChanged
    }

    class SchematicsEntity {
        +Id
        +Type
        +Data
        +Color
        +IsVisible
        +DocumentId
        +IsSelected
    }

    class GeneralSettings {
        +AutoZoomSelectionSchematics
        +WireColorCodes
        +ComponentIdentifierGroups
    }

    class KblMapper {
        +HarnessPartNumber
        +KBLOccurrenceMapper
        +GetOccurrenceObject()
    }

    class OverallConnectivity {
        +HarnessPartNumber
        +HarnessConnectivities
        +InlinerCavityPairs
    }

    SchematicsView --> MainForm: References
    SchematicsView --> AdvConnectivityView: Contains
    SchematicsView --> SchematicsEntity: Manages collection of
    SchematicsView --> GeneralSettings: Uses
    SchematicsView --> OverallConnectivity: Uses
    
    SchematicsView ..> DocumentForm: Interacts with
    SchematicsView ..> KblMapper: Uses
```

#### Key Interaction Patterns

##### Logical Connection Visualization
- **SchematicsView** represents the logical electrical connections between components
- Presents a schematic, abstracted view of harness connectivity rather than physical layout
- Allows users to understand electrical relationships independent of physical routing

##### Document Integration and Management
1. **Multi-Document Handling**:
   - Manages schematic representations for multiple documents simultaneously
   - `AddDocumentToAdvConnectivityView()` integrates a new document's connectivity data
   - `RemoveDocumentFromConnectivityView()` cleanly removes a document when closed

2. **Active Entity Filtering**:
   - ActiveEntities collection maintains currently visible/active entities
   - Applies filtering based on document state, user selection, and visibility settings
   - Updates when documents or selections change

##### Entity Selection Synchronization
- `SelectEntities()` highlights schematic entities based on selections in other views
- Propagates selection changes from schematic view to other views via events
- Maintains consistent selection state across all application views

##### KBL Data Transformation
- Transforms KBL (physical harness description) into logical schematic representation
- Maps between physical components and their logical representation
- Uses KblMapper to access and interpret underlying harness data

##### Visual Styling and Configuration
- Applies wire colors and styling based on GeneralSettings configuration
- Respects user preferences for visual appearance and diagram layout
- Implements auto-zoom functionality based on user settings

##### Export Capabilities
- `ExportToPDF()` generates PDF exports of the schematic view
- Preserves visual styling, layout, and selection state in exports
- Supports documentation and reporting workflows

##### Performance Optimization
- Implements on-demand rendering and entity management
- Uses efficient data structures for entity lookup (`ContainsItem()`)
- Optimizes update operations to minimize UI thread impact

### D3DController Dependency Diagram

```mermaid
classDiagram
    class D3DController {
        +MainFormController
        +DisplayConsolidatedViewAction()
        +LoadCarTransformationByUserAction()
        +SaveCarTransformationAction()
        +Export3DModelAction()
        +UpdateFromGeneralSettings()
        +OnBeforeOpenDocument()
        +OnAfterDocumentClosed()
        +Consolidated3DView
    }

    class MainForm {
        +ActiveDocument
        +DiameterSettings
        +GeneralSettings
        +TopologyHub
        +SchematicsView
        +D3D
    }

    class DocumentForm {
        +_D3DControl
        +Document
        +KBL
        +SelectIn3DView()
        +IsDocument3DActive
    }

    class Document3DControl {
        +Model3DControl1
        +CloseAllToolTips()
        +SelectEntitiesByKbl()
        +ZoomFitSelection()
        +Event SelectionChanged
    }

    class Consolidated3DControl {
        +Design
        +LoadCarModel()
        +SetCarTransformation()
        +SaveCarTransformation()
        +Export3DModel()
    }

    class HcvDocument {
        +Entities
        +Kbl
        +Model
        +IsOpen
        +RecalculateBundleDiameters()
    }

    class DiameterSettings {
        +Diameters
        +GenericDiameterFormulaParameters
        +RawBundleInstallationAddOnTolerance
    }

    class GeneralSettings {
        +ShowFullScreenAxisCursor
        +BundleDiameterRanges
        +CarModelsDirectory
    }

    class CarTransformationSettings {
        +AddOrReplace()
        +Load()
        +Save()
    }

    class XhcvFile {
        +CarTransformation
    }

    D3DController --> MainForm: References
    D3DController --> Consolidated3DControl: Manages
    D3DController --> DiameterSettings: Uses
    D3DController --> GeneralSettings: Uses
    D3DController --> CarTransformationSettings: Uses
    
    D3DController ..> DocumentForm: Interacts with
    D3DController ..> Document3DControl: Interacts with
    D3DController ..> HcvDocument: Uses
    D3DController ..> XhcvFile: Loads from/Saves to
```

#### Key Interaction Patterns

##### 3D Visualization Management

- **D3DController** orchestrates 3D visualization of harness designs using Eyeshot/DevDept components
- Manages both per-document 3D views and consolidated multi-harness 3D views
- Handles rendering configuration, camera control, and selection visualization

##### Dual Viewing Modes

1. **Document-Specific 3D View**:
   - Each DocumentForm contains a Document3DControl for 3D visualization
   - Shows physical routing and component placement for a single harness
   - Synchronized with other document views (2D drawing, information grids)

2. **Consolidated Vehicle View**:
   - `DisplayConsolidatedViewAction()` shows multiple harnesses in vehicle context
   - Uses car model for reference and properly positions harnesses within vehicle
   - Provides whole-vehicle perspective for understanding harness interactions

##### Bundle Diameter Visualization

- Uses DiameterSettings to calculate and visualize proper bundle diameters
- Applies visual styling (colors, thickness) based on bundle properties
- Updates visualization when diameter settings or calculations change

##### Car Model Integration

- `LoadCarTransformationByUserAction()` loads and positions car model geometry
- Manages the correct transformation of car model relative to harnesses
- `SaveCarTransformationAction()` persists positioning for future sessions

##### Selection Synchronization

- Coordinates with other views to maintain consistent selection state
- When items are selected in other views, highlights corresponding 3D entities
- Propagates selections made in 3D view to other components

##### Export Capabilities

- `Export3DModelAction()` generates exports in various 3D formats
- Supports documentation and CAD integration workflows
- Preserves structure, materials, and metadata during export

##### Document Lifecycle Integration

- `OnBeforeOpenDocument()` prepares 3D environment for new document
- `OnAfterDocumentClosed()` cleanly removes 3D resources when document closes
- Manages 3D resource lifecycle to prevent memory leaks

##### Settings Integration

- `UpdateFromGeneralSettings()` applies visual preferences from application settings
- Applies changes to visual style, rendering quality, and UI behavior
- Manages user preferences for 3D visualization (axis display, navigation controls)

These additional descriptions provide developers with a deeper understanding of how each component functions within the application architecture, their key responsibilities, and how they interact with other components. The information focuses on patterns and concepts rather than implementation details, making it useful for onboarding new developers or reviewing the system design.

## > Module: E3.HarnessAnalyzer.Project

- The E3.HarnessAnalyzer.Project module serves as the core project management and document handling framework for the E3.HarnessAnalyzer application. 
- It provides a structured approach to managing harness documents, their associated models, and 3D visualization entities. 
- The module bridges the gap between file I/O operations and the business logic of harness analysis.

### Key Components

```mermaid
classDiagram
    class BaseProject~TDocument~ {
        +Documents: BaseHcvDocumentCollection~TDocument~
        +DoWorkChunksAsync()
        +Invalidate()
        +Event ProjectStateChanging
        +Event ProjectStateChanged
        +Event DocumentStateChanging
        +Event DocumentStateChanged
    }
    
    class BaseHcvDocumentCollection~TDocument~ {
        +Project: BaseProject~TDocument~
        -GetNewFileInstanceCore()
    }
    
    class HcvDocument {
        +Entities: DocumentEntitiesCollection
        +Kbl: KblMapper
        +Model: EESystemModel
        +SchemaVersion: KblSchemaVersion
        +VariantUsedToActivate: Variant
        +LoadedContent: LoadContentType
        +IsXhcv: bool
        +IsLastOfXhcv: bool
        +CanRecalculateBundleDiameters()
        +RecalculateBundleDiameters()
        +SetModelObjectsActiveByKblIds()
        +HasState()
        +ShowAsNewTemporalEntities()
        +Event EntitiesUpdated
    }
    
    class DocumentEntitiesCollection {
        +GetByKblIds()
        +GetByEEObjectId()
        +Invalidate()
        +RegenAsync()
        +Event Updated
    }
    
    class AsyncBundleRecalculator {
        +CanRecalculate()
        +RecalculateBundleDiameters()
        -UpdateBundleDiameters()
        -CalculateSegmentDiameter()
        +Event ProgressChanged
    }
    
    class TemporalEntitiesResult {
        +Document: HcvDocument
        +Entities: MeshEx[]
        +Selected: bool
        +AddAndRegen()
        +AddAsTempEntitiesAndRegen()
        +Regen()
        +ZoomFit()
        +Clear()
    }
    
    class DiameterCalculation {
        +CalculateSegmentDiameter()
        +CalculateSegmentDiameters()
        +GetBundleWithCableAndWireCircles()
        +GetCalculatedOutsideDiameter()
        +GetGenericCoreOrWireDiameter()
    }
    
    class WorkUnitWrappedWorkChunk {
        +Work: IWorkChunk
        +IsCancelled: bool
        +Maximum: long
        +View: IWorkFileViewAdapter
    }
    
    class DocumentWorkUnitWrappedWorkChunk {
        +ChunkType: DocumentWorkChunkType
        #OnReportProgress()
    }
    
    class ContentSettings {
        +LengthClass: LengthClass
        +UseKblAbsoluteLocations: bool
        +JT: JTSettings
    }
    
    BaseProject o-- BaseHcvDocumentCollection : Contains
    BaseHcvDocumentCollection o-- HcvDocument : Contains
    HcvDocument o-- DocumentEntitiesCollection : Contains
    HcvDocument o-- AsyncBundleRecalculator : Uses
    HcvDocument ..> TemporalEntitiesResult : Creates
    HcvDocument ..> DiameterCalculation : Uses
    HcvDocument ..> ContentSettings : Uses
    HcvDocument ..> WorkUnitWrappedWorkChunk : Uses
    WorkUnitWrappedWorkChunk <|-- DocumentWorkUnitWrappedWorkChunk : Extends
```

### Core Classes and Their Roles

1. **BaseProject<TDocument>**: 
   - The root container for all document management
   - Manages document collections and project-wide state
   - Provides event handling for project and document state changes
   - Handles project-level tasks through work chunks

2. **BaseHcvDocumentCollection<TDocument>**:
   - Collection of documents within a project
   - Manages document lifecycle (creation, opening, closing)
   - References back to the parent project

3. **HcvDocument**:
   - Represents a single harness document (based on HCV/KBL files)
   - Contains both document model (KBL) and visualization entities
   - Manages document state and operations
   - Handles bundle diameter calculations, model visualization, and entity management

4. **DocumentEntitiesCollection**:
   - Collection of 3D visualization entities for a document
   - Provides querying by ID and entity type
   - Handles entity rendering and updates

5. **AsyncBundleRecalculator**:
   - Performs bundle diameter calculations asynchronously
   - Reports progress and manages calculation workflow
   - Provides thread-safe operations for UI responsiveness

6. **TemporalEntitiesResult**:
   - Encapsulates a collection of temporary visualization entities
   - Manages entity lifecycle (add, display, remove)
   - Provides operations like selection, highlighting, and zooming

7. **DiameterCalculation**:
   - Static utility class for calculating wire bundle diameters
   - Provides various diameter calculation algorithms
   - Used by AsyncBundleRecalculator for bundle sizing

8. **WorkUnitWrappedWorkChunk / DocumentWorkUnitWrappedWorkChunk**:
   - Encapsulates document processing tasks
   - Manages progress reporting and cancellation
   - Provides view adapters for visualization

### Workflow and Interaction Sequence (HcvDocument)

This sequence diagram illustrates the complete document lifecycle in the E3.HarnessAnalyzer application, showing how a harness document (using HcvDocument - 3D entities) is processed from opening to closing.

```mermaid
sequenceDiagram
    participant MainForm
    participant MainStateMachine
    participant BaseProject
    participant HcvDocument
    participant DocumentEntities
    participant BundleRecalculator
    participant DiameterCalc
    
    MainForm->>MainStateMachine: OpenDocument()
    MainStateMachine->>BaseProject: AddDocument(hcvFile)
    BaseProject->>HcvDocument: Create(hcvFile)
    
    Note over HcvDocument: Document Creation Phase
    
    HcvDocument->>HcvDocument: Initialize()
    HcvDocument->>DocumentEntities: Create()
    
    Note over HcvDocument: Document Opening Phase
    
    MainStateMachine->>HcvDocument: DoWork(OpenWorkChunk)
    HcvDocument->>HcvDocument: OpenCore()
    HcvDocument->>HcvDocument: LoadKbl()
    HcvDocument->>HcvDocument: CreateModel()
    
    Note over HcvDocument: Content Loading Phase
    
    HcvDocument->>HcvDocument: LoadContent(Model | Entities | JTData)
    HcvDocument->>DocumentEntities: BuildFrom(model)
    
    Note over HcvDocument: Post-Processing Phase
    
    MainForm->>HcvDocument: RecalculateBundleDiameters()
    HcvDocument->>BundleRecalculator: RecalculateBundleDiameters()
    BundleRecalculator->>DiameterCalc: CalculateSegmentDiameters()
    DiameterCalc->>DiameterCalc: GetBundleWithCableAndWireCircles()
    DiameterCalc->>DiameterCalc: GetCalculatedOutsideDiameter()
    DiameterCalc-->>BundleRecalculator: Return diameters
    BundleRecalculator->>BundleRecalculator: UpdateBundleDiameters()
    BundleRecalculator->>DocumentEntities: RegenAsync()
    BundleRecalculator-->>HcvDocument: Return result
    HcvDocument-->>MainForm: Notify completion
    
    Note over MainForm, HcvDocument: Working with Document
    
    MainForm->>HcvDocument: SetModelObjectsActiveByKblIds()
    HcvDocument->>DocumentEntities: Update visibility
    
    MainForm->>HcvDocument: ShowAsNewTemporalEntities()
    HcvDocument->>TemporalEntitiesResult: Create(entities)
    TemporalEntitiesResult->>TemporalEntitiesResult: AddAsTempEntitiesAndRegen()
    TemporalEntitiesResult-->>HcvDocument: Return result
    HcvDocument-->>MainForm: Return visualization
    
    Note over MainForm, HcvDocument: Document Closing Phase
    
    MainForm->>MainStateMachine: CloseDocument()
    MainStateMachine->>HcvDocument: DoWork(CloseWorkChunk)
    HcvDocument->>HcvDocument: CloseCore()
    HcvDocument->>DocumentEntities: Clear()
    HcvDocument-->>MainStateMachine: Return result
    MainStateMachine-->>MainForm: Notify completion
```

### Diagram explanation (Workflow)

1.	The workflow is organized into distinct phases that follow a logical progression:
    - Document Creation - Initial instantiation of objects and data structures
    - Document Opening - Loading raw KBL data and constructing the data model
    - Content Loading - Building visual entities from the model
    - Post-Processing - Performing calculations like bundle diameter analysis
    - Working with Document - Interacting with the loaded document
    - Document Closing - Proper cleanup of resources

2.	Key data flow patterns include:
    - The MainForm delegates document operations to the MainStateMachine
    - The HcvDocument serves as the central document model
    - Specialized calculations are delegated to purpose-specific components
    - Results flow back through the same chain of components

3.	The bundle diameter calculation workflow illustrates an important technical process:
    - It traverses from UI through document model to specialized calculators
    - BundleRecalculator coordinates the overall process
    - DiameterCalc performs the actual engineering calculations
    - Results are applied to the visual model via DocumentEntities

### Implementation Details

#### State Management

HcvDocument uses a state machine pattern to track its lifecycle:
- States include None, New, Opening, Open, Closing, Closed
- Operations are guarded by state checks to prevent invalid operations
- Asynchronous operations use locks to prevent state corruption

#### Multi-threading Considerations

- Asynchronous operations use semaphores and locks to ensure thread safety
- Progress reporting is channeled through event handlers
- Long-running operations like bundle recalculation run on background threads
- UI updates are marshaled back to the UI thread

#### Entity Management

- 3D entities are managed through the DocumentEntitiesCollection
- Entities are linked to model objects via ID mappings
- Temporary entities use a disposal pattern to ensure proper cleanup
- Entity invalidation and regeneration is optimized to minimize rendering cost

#### KBL Integration

- KBL data is loaded from the HCV file and parsed into a KblMapper
- The KblMapper provides access to the KBL model and its objects
- Model objects are converted to 3D entities based on their type and properties
- The document maintains bidirectional mappings between KBL IDs and entities

## > Module: E3.HarnessAnalyzer.Settings

- The E3.HarnessAnalyzer.Settings module serves as the central configuration management system for the E3.HarnessAnalyzer application. 
- It provides structured classes for storing, validating, and accessing various types of settings that control the behavior of the application across its many features and views.

### Key Components and Architecture

```mermaid
classDiagram
    class IHarnessAnalyzerSettingsProvider {
        <<interface>>
        +DiameterSettings
        +GeneralSettings
        +QMStampSpecifications
        +HasView3DFeature
        +HasSchematicsFeature
        +HasTopoCompareFeature
    }
    
    class GeneralSettingsBase {
        <<abstract>>
        +BundleDiameterRanges
        +Tutorials
        +RecentFiles
        +WireColorCodes
        +InlinerIdentifiers
        +SpliceIdentifiers
        +EyeletIdentifiers
        +ComponentIdentifierGroups
        +UltrasonicSpliceIdentifiers
        +EcuConnectorIdentifier
        +Save()
        +Load()
        +ResetToDefaults()
        +OnModified()
    }
    
    class GeneralSettings {
        +Current : GeneralSettingsResult
        -CreateComponentIdentifierGroupsInstance()
    }
    
    class DiameterSettings {
        +Diameters : DiameterList
        +GenericDiameterFormulaParameters
        +IsAddOnToleranceOnArea
        +RawBundleInstallationAddOnTolerance
        +RawBundleProvisioningAddOnTolerance
        +Save()
        +Load()
        +ResetToDefaults()
    }
    
    class QMStampSpecifications {
        +Connector : QMStampSpecificationCollection
        +Fixing : QMStampSpecificationCollection
        +Protection : QMStampSpecificationCollection
        +Segment : QMStampSpecificationCollection
        +Unspecified : QMStampSpecificationCollection
        +SaveTo()
        +ResetToDefaults()
    }
    
    class WeightSettings {
        +Weights : WeightsCollection
        +MaterialSpecField
        +MaterialSpecs : MaterialSpecList
        +GenericInsulationWeightParameters
        +CopperFallBackEnabled
        +SaveTo()
        +ResetToDefaults()
    }
    
    class CheckedCompareResultInformation {
        +CheckedCompareResults : CheckedCompareResultList
        +CreatedBy
        +CreatedOn
        +HarnessPartNumber
        +HarnessVersion
        +Save()
        +Load()
    }
    
    class ViewPortSizeSettings {
        +Add()
        +Clear()
        +CalculateForEach()
        +GetCalculatedViewportBounds()
        +SaveTo()
    }
    
    class CarTransformationSettings {
        +AddOrReplace()
    }
    
    class IdentifierBase {
        <<abstract>>
        +KBLPropertyName
        +IdentificationCriteria
        +IsMatch()
    }
    
    class Condition {
        +Type : ConditionType
        +IsMatch()
    }
    
    class MainForm {
        -_diameterSettings : DiameterSettings
        -_generalSettings : GeneralSettings
        -_qmStampSpecifications : QMStampSpecifications
        -_weightSettings : WeightSettings
        -InitializeDiameterSettings()
        -InitializeWeightSettings()
        -InitializeStampSpecifications()
    }
    
    GeneralSettingsBase <|-- GeneralSettings
    IHarnessAnalyzerSettingsProvider <|.. MainForm
    
    MainForm --> GeneralSettings
    MainForm --> DiameterSettings
    MainForm --> QMStampSpecifications
    MainForm --> WeightSettings
    
    GeneralSettingsBase --> IdentifierBase
    IdentifierBase <|-- InlinerIdentifier
    IdentifierBase <|-- SpliceIdentifier
    IdentifierBase <|-- EyeletIdentifier
    IdentifierBase <|-- EcuConnectorIdentifier
    
    IdentifierBase --> Condition
    
    GeneralSettingsBase --> BundleDiameterRange
    DiameterSettings --> Diameter
    DiameterSettings --> GenericDiameterFormulaParameters
    WeightSettings --> NumericValue
    NumericValue <|-- WeightNumericValue
```

### Core Settings Classes

1. **GeneralSettingsBase / GeneralSettings**
   - Abstract base class and concrete implementation for application-wide settings
   - Manages UI preferences, visual display options, file paths, and identifiers
   - Handles loading/saving of settings to XML files

2. **DiameterSettings**
   - Specialized settings for bundle diameter calculations
   - Contains formulas and parameters for diameter calculations
   - Maintains a list of predefined wire diameters and tolerance settings

3. **QMStampSpecifications**
   - Quality management stamp specifications
   - Organized by component type (connector, fixing, protection, segment)
   - Used for quality control and validation

4. **WeightSettings**
   - Parameters for calculating weights of harness components
   - Includes material specifications and conversion formulas
   - Supports different material types and their physical properties

### 3D Visualization Support

The settings module provides configuration for 3D visualization:

1. **ViewPortSizeSettings** - Controls the size and position of viewports
2. **CarTransformationSettings** - Stores transformation matrices for car models
3. **BundleDiameterRange** - Defines diameter ranges and their visual representation

### Usage Flow in Application

- This sequence diagram illustrates the settings management workflow in the E3.HarnessAnalyzer application. 
- It shows how configuration data is loaded, applied, and persisted throughout the application lifecycle.

```mermaid
sequenceDiagram
    participant App as Application
    participant MainForm
    participant SettingsLoader as Settings Loader
    participant GenSettings as GeneralSettings
    participant DiamSettings as DiameterSettings
    participant QMSettings as QMStampSpecifications
    participant UI as UI Components

    App->>MainForm: Create MainForm
    MainForm->>MainForm: Initialize()
    
    MainForm->>SettingsLoader: Load Settings
    SettingsLoader->>GenSettings: LoadFromFile()
    SettingsLoader->>DiamSettings: LoadFromFile()
    SettingsLoader->>QMSettings: LoadFromFile()
    
    alt Settings file exists
        GenSettings-->>MainForm: Return loaded settings
    else Settings file doesn't exist
        GenSettings->>GenSettings: ResetToDefaults()
        GenSettings-->>MainForm: Return default settings
    end
    
    MainForm->>UI: Apply settings to UI components
    
    Note over MainForm,UI: User makes changes to settings
    
    UI->>MainForm: Settings changed
    MainForm->>GenSettings: Update settings
    
    MainForm->>GenSettings: Save()
    GenSettings->>SettingsLoader: Save to XML file
    
    Note over App,UI: Settings affect application behavior
    UI->>GenSettings: Access settings values
    GenSettings-->>UI: Return current settings
    UI->>App: Apply behavior based on settings
```

### Usage Flow diagram explanation

1.	Initialization phase where D3DController loads general settings and car model configurations
2.	How car transformation settings are applied to position vehicle models correctly in 3D space
3.	The bundle diameter calculation workflow, where settings provide the formulas and parameters
4.	How visual styling (colors and rendering properties) is applied to bundles based on diameter ranges

### Integration with 3D Components

- This sequence diagram illustrates the data flow between the Settings module and 3D visualization components in the E3.HarnessAnalyzer application. 
- It shows how configuration data from Settings controls the appearance and behavior of 3D models.

```mermaid
sequenceDiagram
    participant MainForm
    participant Settings as E3.HarnessAnalyzer.Settings
    participant D3DController as D3D Controller
    participant Consolidated3D as Consolidated3DControl
    participant Doc3D as Document3DControl
    
    MainForm->>D3DController: Initialize(settings)
    D3DController->>Settings: Access GeneralSettings
    D3DController->>Consolidated3D: Initialize(carModelsDirectory)
    
    Note over Consolidated3D: 3D view settings applied
    
    Consolidated3D->>Settings: Access CarTransformationSettings
    Consolidated3D->>Consolidated3D: Apply transformation
    
    MainForm->>D3DController: RecalculateBundleDiameters()
    D3DController->>Settings: Access DiameterSettings
    D3DController->>Doc3D: RecalculateBundleDiameters(diameterSettings)
    Doc3D->>Doc3D: Apply bundle calculations
    
    Note over MainForm,Doc3D: Bundle colors applied based on settings
    Doc3D->>Settings: Access BundleDiameterRanges
    Doc3D->>Doc3D: Apply colors based on diameter ranges
```

### Sequence diagram explanation

1.	The bidirectional relationship between XML files and settings classes, enabling persistent storage and retrieval of user preferences
2.	The various settings categories (GeneralSettings, DiameterSettings, etc.) that control different aspects of the application
3.	How these settings provide configuration to the application's core components (MainForm, 3D Visualization, etc.)
4.	The circular flow where user interface actions modify settings, which then update the application behavior, which in turn refreshes the UI

### Settings Classes and Their Relationships

The settings module follows a consistent pattern for all settings types:

1. **Loading** - Static Load/LoadFromFile methods to deserialize from XML
2. **Defaults** - ResetToDefaults method to provide default values
3. **Saving** - Save/SaveTo methods to serialize to XML
4. **Notification** - Event-based notification when settings change

This pattern ensures consistent behavior across all settings types and simplifies the integration with the main application.

### Configuration Data Flow

- This flowchart illustrates the configuration data flow architecture in the E3.HarnessAnalyzer application. 
- It depicts how settings are stored, accessed, and propagated throughout the system.

```mermaid
flowchart TD
    XML[XML Files] <-->|Load/Save| SettingsClasses[Settings Classes]
    SettingsClasses -->|Provides Configuration| Application[Application Components]
    User[User Interface] -->|Modifies| SettingsClasses
    Application -->|Updates| User
    
    subgraph SettingsClasses
        GeneralSettings
        DiameterSettings
        QMStampSpecifications
        WeightSettings
        ViewPortSettings[ViewPortSizeSettings]
        CarSettings[CarTransformationSettings]
    end
    
    subgraph Application
        MainForm
        D3D[3D Visualization]
        Schematics[Schematic Views]
        Compare[Compare Tools]
        Document[Document Management]
    end
```

### Configuration data flow diagram explanation

1.	The bidirectional relationship between XML files and settings classes, enabling persistent storage and retrieval of user preferences
2.	The various settings categories (GeneralSettings, DiameterSettings, etc.) that control different aspects of the application
3.	How these settings provide configuration to the application's core components (MainForm, 3D Visualization, etc.)
4.	The circular flow where user interface actions modify settings, which then update the application behavior, which in turn refreshes the UI

## > Module: E3.HarnessAnalyzer.Shared

- The E3.HarnessAnalyzer.Shared module provides fundamental utilities, interfaces, and common functionality used across the E3.HarnessAnalyzer application. 
- It serves as the foundation for consistent behavior and code reuse across the application's various components.

### Core Architecture

```
classDiagram
    class Common {
        <<module>>
        +Constants
        +Enumerations
        +Shared fields
    }
    
    class Utilities {
        +GetApplicationSettingsPath()
        +GetOrCreateApplicationDirectoryName()
        +ConvertBooleanToString()
        +ConvertToBoolean()
        +ConvertToSingle()
        +ReadXml~T~()
        +WriteXml()
    }
    
    class Extensions {
        <<extension methods>>
        +AddOrUpdate()
        +TryAdd()
        +AddNewOrGet()
        +ForEach()
        +GetFlags()
        +DrawOverlay()
    }
    
    class LicenseManager {
        +InitAndAuthenticate()
        +AuthFeaturesOrAvailable()
        +EnvironmentVariables
        +LmBorrowExecutablePath
    }
    
    class ICollectionGrouping~TKey,TElement~ {
        <<interface>>
        +Add()
        +Remove()
    }
    
    class IKblDistanceEntry {
        <<interface>>
        +Start
        +End
        +KblId
        +KblSystemId
    }
    
    class DictionaryGroupedValues~TKey,TValue,TGroup~ {
        +AsIDictionary()
        +AsGroups()
    }
    
    class ApplicationFeature {
        <<enumeration>>
        E3HarnessAnalyzer
        E3HarnessAnalyzerDSI
        E3HarnessAnalyzer3D
        E3HarnessAnalyzerPlmXml
        E3HarnessAnalyzerConBrow
        E3HarnessAnalyzerTopoCompare
    }
    
    class ToastMessageVerbosity {
        <<enumeration>>
        Silent
        Warnings
        Errors
    }
    
    Common --> ApplicationFeature: Defines
    LicenseManager --> ApplicationFeature: Uses
    Utilities --> Common: Uses constants
    Extensions --> Common: Extends functionality
    DictionaryGroupedValues --> ICollectionGrouping: Implements for groups
    
    note for Common "Central location for constants and enumerations"
    note for Utilities "Core utility functions for file and data handling"
    note for Extensions "Extension methods to enhance base .NET types"
    note for LicenseManager "Manages application licensing and feature authentication"
```
    
### Core Components

#### Constants and Enumerations (Common.vb)

The `Common` module serves as a central repository for constants, enumerations, and shared fields used throughout the application:

```vb
Public Const ISACTIVE_VARIANT_NAME As String = "DocumentActivatedVariant"
Public Const CHANGED_MODIFIED_FORECOLOR As Drawing.KnownColor = Drawing.KnownColor.DarkOrange
Public Const CAR_TRANSFORMATION_SETTING_xHCVFILE As String = "CarTransformationSetting.bin"
Public Const COMPANY_FOLDER As String = "Zuken"
Public Const PRODUCT_FOLDER As String = "E3.HarnessAnalyzer"
```

Key enumerations include:
- `ApplicationFeature` - Defines licensable features in the application
- `ApplicationMenuToolKey`, `HomeTabToolKey`, etc. - Organize UI components
- `ToastMessageVerbosity` - Controls notification verbosity levels
- `PaneKeys` - Identifies dockable panes in the application

#### License Management (LicenseManager.vb)

> Note: for general deeper dive into the licensing library please refer to the [Lib.Licensing developer wiki](https://ulm-dev.zuken.com/Team-Erlangen/E3.Lib.Licensing/wiki/E3.Lib.Licensing-developer-documentation) and the [Lib.Licensing README.md](https://ulm-dev.zuken.com/Team-Erlangen/E3.Lib.Licensing/src/branch/trunk/README.md)
    
The `LicenseManager` class handles feature licensing and authentication:

```vb
Public Class LicenseManager
    Inherits LicenseManager(Of ApplicationFeature)
    
    Public Const VENDOR_ZUKEN As String = "zuken"
    
    Public Function InitAndAuthenticate(Optional features() As String = Nothing, Optional createRegKeys As Boolean = False) As Result
        ' Implementation
    End Function
    
    Public Function AuthFeaturesOrAvailable(ParamArray features() As String) As AuthFeaturesResult
        ' Implementation
    End Function
    
    Public Overrides ReadOnly Property ProductName As String
        Get
            Return "E3.HarnessAnalyzer"
        End Get
    End Property
```

### Integration With Other Modules

The Shared module integrates with other components through several interfaces and utilities:

```mermaid
flowchart TD
    Shared[E3.HarnessAnalyzer.Shared]
    MainApp[E3.HarnessAnalyzer]
    Project[E3.HarnessAnalyzer.Project]
    Settings[E3.HarnessAnalyzer.Settings]
    Compatibility[E3.HarnessAnalyzer.Compatibility]
    BorrowUtil[BorrowUtility]
    
    Shared --> MainApp
    Shared --> Project
    Shared --> Settings
    Shared --> Compatibility
    Shared --> BorrowUtil
    
    subgraph SharedComponents[Key Shared Components Used Across Modules]
        direction TB
        Constants["Constants & Paths"]
        Features["Feature Definitions"]
        Extensions["Collection Extensions"]
        LicenseManager["License Management"]
        Utilities["File & XML Utilities"]
    end
    
    Constants -.-> MainApp
    Constants -.-> Project
    Constants -.-> Settings
    Constants -.-> Compatibility
    Constants -.-> BorrowUtil
    
    Features -.-> MainApp
    Features -.-> BorrowUtil
    
    Extensions -.-> Project
    Extensions -.-> Settings
    
    LicenseManager -.-> MainApp
    LicenseManager -.-> BorrowUtil
    
    Utilities -.-> Settings
    Utilities -.-> Compatibility
```

### Diagram explanation

1.	The hierarchical dependency structure where the Shared module is used by all other major modules (MainApp, Project, Settings, Compatibility, and BorrowUtil)
2.	The internal organization of the Shared module into specialized components:
    - Constants & Paths provide consistent values across the application
    - Feature Definitions define licensable features and capabilities
    - Collection Extensions offer reusable collection manipulation methods
    - License Management handles authentication and feature authorization
    - File & XML Utilities provide standardized I/O operations
3.	The specific usage patterns where certain modules only leverage relevant components from Shared:
    - License management is primarily used by the main app and BorrowUtility
    - Collection extensions support Project and Settings modules
    - File utilities are mainly used by Settings and Compatibility

#### Integration Examples

1. **With MainForm**:
   - Uses constants for window titles, logging levels
   - Leverages license management for feature availability checks

2. **With Project**:
   - Provides extension methods for collection manipulation
   - Defines interfaces for document components and entities

3. **With Settings**:
   - Supplies path utilities for reading/writing configuration
   - Provides XML serialization helpers

4. **With BorrowUtility**:
   - Shares license management infrastructure
   - Defines available features through ApplicationFeature enum

### Data Flow and Usage Patterns

#### License Authentication Flow

This sequence diagram illustrates how the E3.HarnessAnalyzer application validates its license features during startup:

```mermaid
sequenceDiagram
    participant App as Application
    participant LM as LicenseManager
    participant AuthMgr as E3Auth (Native)
    participant RegKeys as Registry Keys
    
    App->>LM: InitAndAuthenticate()
    LM->>LM: Init(createRegKeys)
    LM->>AuthMgr: Initialize(AppVersionMain)
    
    alt Registry has product key
        LM->>RegKeys: Read available features
    else Registry doesn't have product key
        LM->>LM: Use default features
        
        opt createRegKeys = true
            LM->>RegKeys: Create registry keys
        end
    end
    
    LM->>LM: AuthenticateFeatures(features)
    LM->>AuthMgr: GetLicense(MainFeature)
    
    alt Main feature authenticated
        LM->>AuthMgr: GetLicense for each additional feature
        AuthMgr->>LM: Return success/failure for each
        LM->>App: Return authenticated features
    else Main feature failed
        LM->>App: Return authentication failure
    end
```

#### Diagram explanation (License Authentication Flow)

1.	The authentication process begins when the application calls InitAndAuthenticate() on the LicenseManager
2.	The LicenseManager initializes itself and connects to the native E3Auth component (FlexLM wrapper)
3.	The system checks registry keys to determine which features should be available:
    - If registry entries exist, it reads the available features
    - If not, it falls back to default features and optionally creates registry entries
4.	Authentication happens in a hierarchical order:
    - First, the main (core) feature is validated
    - Only if the main feature is successfully authenticated does it proceed to check additional features
    - Results for each feature are collected and returned to the application

#### Framework-Specific Extensions

This diagram illustrates how the E3.HarnessAnalyzer.Shared module handles cross-framework compatibility through targeted code implementations:

```mermaid
flowchart TD
    Shared[E3.HarnessAnalyzer.Shared]
    
    subgraph FrameworkSpecific[Framework-Specific Code]
        NETFX[".NET Framework Code"]
        NET6[".NET 6/8 Code"]
        Common["Common Code"]
    end
    
    Shared --> NETFX
    Shared --> NET6
    Shared --> Common
    
    NETFX --> IMGEXT["Image Extensions"]
    NETFX --> REGEXT["Registry Extensions"]
    NETFX --> UIEXT["UI Extensions"]
    
    NET6 --> ALTIMG["Alternative Image Handling"]
    NET6 --> ALTREG["Alternative Registry Access"]
    NET6 --> ALTUI["Alternative UI Handling"]
    
    Common --> Collections["Collection Extensions"]
    Common --> Conversions["Data Conversion"]
    Common --> XML["XML Utilities"]
    Common --> Licensing["License Management"]
```

### Diagram explanation (Framework-Specific Extensions)

1.	The architecture uses a framework-detection pattern to provide appropriate implementations based on the runtime environment (.NET Framework vs .NET 6/8)
2.	Framework-specific implementations are organized into distinct code paths:
    - .NET Framework code provides native implementations for Windows-specific features like registry access, image handling, and UI components
    - .NET 6/8 code provides alternative implementations for cross-platform functionality using modern APIs
    - Common code contains framework-agnostic implementations shared across all target platforms
3.	This approach enables:
    - Code reuse through common implementations where possible
    - Specialized optimizations for each framework
    - Clean conditional compilation using preprocessor directives
    - Seamless developer experience regardless of deployment target

### Error Handling and Robustness

The Shared module incorporates several patterns for error handling and robustness:

1. **Defensive Programming**:
   - Null checks in extension methods
   - Default values in conversion functions
   - Safe handling of missing files/directories
2. **Optional Parameters**:
   - Many methods provide optional parameters with sensible defaults
   - This reduces the need for multiple method overloads
3. **Result-Based Return Values**:
   - Methods often return result objects rather than throwing exceptions
   - This allows for more controlled error handling by consumers
4. **Debug Helpers**:
   - Special handling for debugging scenarios
   - Conditional compilation for development-only features

## > Module: E3.HarnessAnalyzer.Compatibility

```mermaid
classDiagram
    class E3_HarnessAnalyzer {
        Main application
        Targets .NET 8
        References Compatibility module
    }
    
    class E3_HarnessAnalyzer_Compatibility {
        Framework compatibility layer
        Targets multiple frameworks (.NET 472, .NET 8)
        Provides serialization compatibility
    }
    
    namespace Printing {
        class Printer_2023 {
            Legacy printer representation
            Serialization support
        }
        
        class PageEx_2023 {
            Legacy page settings
        }
        
        class MarginsEx_2023 {
            Legacy margin settings
        }
        
        class MilliInch_2023 {
            Base measurement class
            Abstract class
        }
        
        class MilliInch96_2023 {
            1/96 inch representation
            Conversion methods
        }
        
        class MilliInch100_2023 {
            1/100 inch representation
            Conversion methods
        }
    }
    
    namespace ViewportSizeSettings {
        class ViewportSizeSetting_2023 {
            Legacy viewport size settings
            Size and location data
        }
        
        class ViewPortSizeSettingCollection_2023 {
            Collection of viewport settings
            Serialization support
        }
    }
    
    namespace CheckedCompareResultInformation {
        class CheckedCompareResult_2023 {
            Legacy comparison result
            Contains reference to entries
        }
        
        class CheckedCompareResultEntry_2023 {
            Individual comparison result entry
            Stores comparison data
        }
        
        class CheckedCompareResultList_2023 {
            Collection of comparison results
            Serialization support
        }
    }
    
    namespace CavityCheckSettings {
        class CavityCheckSettings_2023 {
            Legacy cavity check configuration
            Serialization support
        }
        
        class ConnectorSetting_2023 {
            Connector configuration
        }
        
        class ModuleSetting_2023 {
            Module configuration
        }
        
        class ModuleSettingCollection_2023 {
            Collection of module settings
        }
    }
    
    namespace CartransformationSettings {
        class CarTransformationSetting_2023 {
            Legacy car transformation setting
            Serialization support
        }
        
        class CarTransformationSettings_2023 {
            Collection of car transformation settings
        }
    }
    
    E3_HarnessAnalyzer --> E3_HarnessAnalyzer_Compatibility : References
    
    E3_HarnessAnalyzer_Compatibility --> Printer_2023
    E3_HarnessAnalyzer_Compatibility --> PageEx_2023
    E3_HarnessAnalyzer_Compatibility --> MarginsEx_2023
    E3_HarnessAnalyzer_Compatibility --> MilliInch_2023
    E3_HarnessAnalyzer_Compatibility --> MilliInch96_2023
    E3_HarnessAnalyzer_Compatibility --> MilliInch100_2023
    
    E3_HarnessAnalyzer_Compatibility --> ViewportSizeSetting_2023
    E3_HarnessAnalyzer_Compatibility --> ViewPortSizeSettingCollection_2023
    
    E3_HarnessAnalyzer_Compatibility --> CheckedCompareResult_2023
    E3_HarnessAnalyzer_Compatibility --> CheckedCompareResultEntry_2023
    E3_HarnessAnalyzer_Compatibility --> CheckedCompareResultList_2023
    
    E3_HarnessAnalyzer_Compatibility --> CavityCheckSettings_2023
    E3_HarnessAnalyzer_Compatibility --> ConnectorSetting_2023
    E3_HarnessAnalyzer_Compatibility --> ModuleSetting_2023
    E3_HarnessAnalyzer_Compatibility --> ModuleSettingCollection_2023
    
    E3_HarnessAnalyzer_Compatibility --> CarTransformationSetting_2023
    E3_HarnessAnalyzer_Compatibility --> CarTransformationSettings_2023
    
    MilliInch_2023 <|-- MilliInch96_2023 : Inherits
    MilliInch_2023 <|-- MilliInch100_2023 : Inherits
    
    ViewPortSizeSettingCollection_2023 o-- ViewportSizeSetting_2023 : Contains
    CheckedCompareResult_2023 o-- CheckedCompareResultEntry_2023 : Contains
    CheckedCompareResultList_2023 o-- CheckedCompareResult_2023 : Contains
    CavityCheckSettings_2023 o-- ConnectorSetting_2023 : Contains
    CavityCheckSettings_2023 o-- ModuleSettingCollection_2023 : Contains
    ModuleSettingCollection_2023 o-- ModuleSetting_2023 : Contains
    CarTransformationSettings_2023 o-- CarTransformationSetting_2023 : Contains
```
   
### Purpose and Usage of E3.HarnessAnalyzer.Compatibility
- The E3.HarnessAnalyzer.Compatibility module serves as a cross-platform compatibility layer that allows the E3.HarnessAnalyzer application to work seamlessly across different .NET frameworks (.NET Framework 4.7.2, .NET 6, and .NET 8). 
- This module is critical for the following reasons:
    1. **Framework Migration Support**: It facilitates the migration from .NET Framework 4.7.2 to newer .NET versions (6 and 8) by providing compatibility classes that handle framework-specific differences.
    2. **Serialization Compatibility**: Many classes in this module (with "_2023" suffix) are specifically designed to handle serialization compatibility with data files created by older versions of the application. This allows the newer versions to read and process legacy data files.
    3. **Cross-Platform Operation**: The module enables the application to run on different platforms by abstracting platform-specific functionality.

### Key Components and Their Roles

#### Printing Components
Classes like `Printer_2023`, `PageEx_2023`, and related measurement classes (`MilliInch_2023`, `MilliInch96_2023`, `MilliInch100_2023`) provide compatibility for printing functionality. `Printer2023.vb` and certain printing functionalities are only available on Windows/.NET Framework environments.

#### Configuration Components
Classes like `ViewportSizeSetting_2023`, `CavityCheckSettings_2023`, and `CarTransformationSetting_2023` handle the serialization and deserialization of application settings and configurations. These classes implement `ISerializable` to provide custom serialization behavior, ensuring that settings files created by older versions can be read by newer versions.

#### Data Model Components
Classes like `CheckedCompareResult_2023` and its related classes provide compatibility for data models used in comparison operations within the application.

### Compatibility-Layer Flow Chart

This diagram illustrates how the compatibility classes in E3.HarnessAnalyzer.Compatibility are used within the main application for cross-framework compatibility and serialization support.
    
```mermaid
flowchart TD
    %% Layout Control
    LegacyClasses:::hiddenLinkStyle ~~~ Start
    
    %% Main Application Startup - Centered under Legacy Classes
    Start([Application Start]) --> MainForm[MainForm Initialization]
    MainForm --> LoadSettings[Load Application Settings]
    
    %% Settings Loading Branch
    LoadSettings --> LoadUserSettings[Load User Preferences]
    LoadUserSettings --> CheckFileFormat{Check File Format}
    CheckFileFormat -->|Legacy Format| LoadLegacySettings[Load Using Compatibility Classes]
    CheckFileFormat -->|Current Format| LoadCurrentSettings[Load Using Current Classes]
    
    %% Legacy Settings Loading
    LoadLegacySettings --> ViewPortLoader[ViewPortSizeSettingCollection_2023.Load]
    LoadLegacySettings --> PrinterLoader[Printer_2023.Deserialize]
    
    %% Document Opening Branch
    MainForm --> OpenDocument[Open Document]
    OpenDocument --> CheckDocumentType{Check Document Type}
    CheckDocumentType -->|KBL/XHCV File| LoadDocumentSettings[Load Document Settings]
    
    %% Document Settings Loading
    LoadDocumentSettings --> CheckSettingsFormat{Check Settings Format}
    CheckSettingsFormat -->|Legacy Format| LoadLegacyDocSettings[Load Legacy Document Settings]
    CheckSettingsFormat -->|Current Format| LoadCurrentDocSettings[Load Current Document Settings]
    
    %% Legacy Document Settings Loading
    LoadLegacyDocSettings --> CarTransformLoader[CarTransformationSetting_2023.Load]
    LoadLegacyDocSettings --> CavitySettingsLoader[CavityCheckSettings_2023.LoadFromFile]
    
    %% Working with Comparison Results
    MainForm --> RunComparison[Run Comparison]
    RunComparison --> SaveResults[Save Comparison Results]
    SaveResults --> CheckResultsFormat{Check Results Format}
    CheckResultsFormat -->|Legacy Format| LoadLegacyResults[Use CheckedCompareResult_2023]
    CheckResultsFormat -->|Current Format| SaveCurrentResults[Use Current Result Classes]
    
    %% Printing Operations
    MainForm --> PrintDocument[Print Document]
    PrintDocument --> CheckPrintingSystem{Check Printing System}
    CheckPrintingSystem -->|Legacy System| UseLegacyPrinting[Use Compatibility Printing]
    CheckPrintingSystem -->|Current System| UseCurrentPrinting[Use Current Printing]
    
    %% Legacy Printing Path
    UseLegacyPrinting --> PrintingClasses[Use Legacy Printing Classes]
    PrintingClasses --> PerformLegacyPrint[Print via Compatibility Layer]
    
    %% Framework Detection Flow
    Start --> DetectFramework[Detect Framework Version]
    DetectFramework --> FrameworkSpecificCode{Framework Specific Code}
    FrameworkSpecificCode -->|.NET Framework| UseNetFrameworkClasses[Use .NET Framework Classes]
    FrameworkSpecificCode -->|.NET 6/8| UseNetCoreClasses[Use .NET Core Classes]
    
    %% Framework-specific implementation paths
    UseNetFrameworkClasses --> FrameworkSpecificPrinting[Framework-specific Printing]
    UseNetCoreClasses --> CoreSpecificImplementation[Core-specific Implementation]
    
    %% Legacy Classes Legend - Positioned at the top
    subgraph LegacyClasses ["E3.HarnessAnalyzer.Compatibility Classes"]
        direction TB
        subgraph Printing ["Printing"]
            direction TB
            Printer_2023
            PrinterSettingsEx_2023
            PageEx_2023
            MarginsEx_2023
            MilliInch_2023
            MilliInch96_2023
            MilliInch100_2023
        end
        
        subgraph ViewportClasses ["Viewport"]
            direction TB
            ViewPortSizeSettingCollection_2023
            ViewportSizeSetting_2023
        end
        
        subgraph CompareClasses ["Comparison"]
            direction TB
            CheckedCompareResult_2023
            CheckedCompareResultEntry_2023
            CheckedCompareResultList_2023
        end
        
        subgraph CarClasses ["Car Settings"]
            direction TB
            CarTransformationSetting_2023
            CarTransformationSettings_2023
        end
        
        subgraph CavityClasses ["Cavity Settings"]
            direction TB
            CavityCheckSettings_2023
            ConnectorSetting_2023
            ModuleSetting_2023
            ModuleSettingCollection_2023
        end
    end
```
    
#### Detailed Usage Explanations

##### Settings and Configuration Flow

1. **Application Startup**
   - When E3.HarnessAnalyzer starts, it loads user preferences and application settings
   - The application checks file formats to determine whether to use current or legacy deserializers

2. **ViewPort Settings Loading**
   - Legacy viewport configurations are loaded using `ViewPortSizeSettingCollection_2023.Load()`
   - These settings control the size and positioning of document viewport windows
   - Example path: `ApplicationSettings.LoadViewportSettings() -> ViewPortSizeSettingCollection_2023.Load()`

3. **Printer Settings Handling**
   - Legacy printer settings are deserialized using `Printer_2023.Deserialize()`
   - These contain printer configurations, page setups, and margin information
   - The measurement classes (`MilliInch_2023`, `MilliInch96_2023`, `MilliInch100_2023`) convert between different measurement units

##### Document Processing Flow

1. **Document Opening**
   - When opening a document (KBL/XHCV file), associated settings are also loaded
   - Settings may be in legacy or current format

2. **Car Transformation Settings**
   - For 3D views, car transformation settings define positioning and orientation
   - Legacy settings are loaded via `CarTransformationSetting_2023.Load()`
   - Used in the 3D document controller to position harness components

3. **Cavity Check Settings**
   - Legacy cavity check configurations use `CavityCheckSettings_2023.LoadFromFile()`
   - These settings define connector and module configurations for cavity checking
   - Used in cavity check operations to verify proper connections

##### Comparison Operations

1. **Comparison Results**
   - When comparing documents, results may be stored in legacy format
   - `CheckedCompareResult_2023` and related classes handle legacy comparison data
   - These are used when loading previously saved comparison results

##### Framework-Specific Functionality

1. **Printing Implementation**
   - Printing is highly framework-dependent
   - In .NET Framework, native printing classes are available
   - In .NET Core/6/8, alternative implementations are needed
   - The compatibility layer bridges these differences with conditional compilation

2. **Platform Detection**
   - At runtime, the application detects the framework version
   - Certain features use different implementations based on the detected framework
   - Conditional compilation directives (like `#If NETFRAMEWORK Or WINDOWS7_0_OR_GREATER Then`) control which code paths are compiled

##### Serialization Strategy

The compatibility classes primarily use custom serialization through `ISerializable` interfaces and `SerializationBinder` subclasses to:

1. Map between original type names and compatibility classes
2. Handle structural differences between versions
3. Accommodate changes in namespaces and class structures
4. Provide forward compatibility for data created in older versions

## > Module: BorrowUtility

- The BorrowUtility is a specialized component of the E3.HarnessAnalyzer suite that enables users to borrow FlexLM licenses from a license server for offline use. 
- This utility is essential for users who need to work with the E3.HarnessAnalyzer without continuous network connectivity to a license server.

> Note: for additional informations go to the BorrowUtility [README.md](https://ulm-dev.zuken.com/Team-Erlangen/E3.App.Windows.HarnessAnalyzer/src/branch/trunk/Solution/BorrowUtility/README.md)

### Architecture Overview

```mermaid
classDiagram
    class BorrowUtility {
        -CommandLineSwitch _commandLineSwitch
        -Logger _logger
        -LicenseManager _manager
        -LogViewFrm _logView
        -ServerLicensesDialog _serverLicDlg
        -SemaphoreSlim _lock
        +New()
        -GetCommandLineSwitch()
        -IsAnalyzerRunning()
        -ubtnBorrow_Click()
        -ubtnReturn_Click()
        +LogView
    }
    
    class FlmBorrowHandler {
        -ApplicationFeature _availableFeatures
        -String _vendor
        -String _lastError
        -String _lastWarning
        +GetLastError
        +GetLastWarning
        -GetPlatformPath()
        -ExecuteProcess()
        +GetExpirationDate()
        +SetAvailableFeaturesFromServerLicense()
        +BorrowLicense()
        +ReturnLicense()
        +PurgeUnusedLicenseEntries()
    }
    
    class SigningChecker {
        +IsSigned()
    }
    
    class ServerLicensesDialog {
        -SemaphoreSlim _lock
        -Boolean _isBusy
        -Boolean _internal
        +Logger
        +Manager
        -RefreshFeatures()
        -ReturnLicenseItem()
        -BorrowItem()
        -GetExpirationDateFromUser()
    }
    
    class SelectExpirationDateDialog {
        +OK_Button_Click()
        +Cancel_Button_Click()
    }
    
    class EnvVarsDialog {
        +Manager
        +Variables
        +GetEnvVariables()
        +GetEnvVariablesByTarget()
        +Populate()
    }
    
    class LogViewFrm {
        -Logger _logger
        +Logger
        -_logger_LogChanged()
        -DoLogChanged()
    }
    
    class AvailableFeaturesDlg {
        +Manager
        +CheckedFeatures
    }
    
    class LicenseManager {
        +Borrow
        +Status
        +EnvironmentVariables
        +AvailableFeatures
        +Init()
        +ResetLicensing()
        +SetAvailableFeatures()
    }
    
    class Logger {
        +ToString()
        +Clear()
        +Event LogChanged
    }
    
    class CommandLineSwitch {
        <<enumeration>>
        NoSwitch
        BuildServerTest
        Debug
    }
    
    BorrowUtility --> LicenseManager : Uses
    BorrowUtility --> Logger : Uses
    BorrowUtility --> LogViewFrm : Creates/Uses
    BorrowUtility --> ServerLicensesDialog : Creates/Uses
    BorrowUtility --> CommandLineSwitch : Uses
    
    LicenseManager --> FlmBorrowHandler : Uses internally
    
    ServerLicensesDialog --> SelectExpirationDateDialog : Uses
    ServerLicensesDialog --> Logger : Uses
    
    LogViewFrm --> Logger : Displays
    
    FlmBorrowHandler --> SigningChecker : Uses for validation
    
    BorrowUtility ..> AvailableFeaturesDlg : Opens
    BorrowUtility ..> EnvVarsDialog : Opens
    
    note for BorrowUtility "Main UI for license borrowing operations"
    note for FlmBorrowHandler "Core implementation of FlexLM borrowing commands"
    note for SigningChecker "Validates executable signatures for security"
    note for LicenseManager "Abstracts licensing operations for the application"
```

### How the BorrowUtility Works (Developer Perspective)

The BorrowUtility follows a layered architecture:

1. **UI Layer**: BorrowUtility.vb serves as the main form, providing UI controls for borrowing/returning licenses and displaying their status.

2. **Business Logic Layer**: 
   - The `LicenseManager` class abstracts the licensing operations.
   - `FlmBorrowHandler` provides the core implementation, interacting with FlexLM executables.

3. **Security Layer**: 
   - `SigningChecker` validates executable signatures before running external processes.

4. **Integration Layer**:
   - The utility interacts with FlexLM via command-line executables (lmborrow.exe, lmstat.exe).
   - A special helper executable (BorrowHelper.exe) facilitates the checkout process.

#### Key Operations Flow

1. **Borrowing a License**:
   - UI triggers `ubtnBorrow_Click` in BorrowUtility.vb
   - App validates the chosen expiration date
   - Queries available features from the server via `_manager.Status.GetAvailableFeaturesFromServer`
   - Borrows the license via `_manager.Borrow.BorrowFeaturesOrAvailable`
   - FlmBorrowHandler executes the lmborrow.exe with appropriate parameters
   - BorrowHelper.exe completes the checkout process
   - Status is updated in the UI

2. **Returning a License**:
   - UI triggers `ubtnReturn_Click` in BorrowUtility.vb
   - Manager returns all features from all servers via `_manager.Borrow.ReturnAllFeaturesFromAllServers`
   - FlmBorrowHandler executes lmborrow.exe with -return parameter
   - Status is updated in the UI

3. **License Status Management**:
   - UI displays current license status during load via `_manager.Borrow.GetExpirationDate`
   - ServerLicensesDialog provides detailed view of available licenses

### Sequence of Borrowing Operation

This sequence diagram illustrates the complete license borrowing process in the BorrowUtility application.

```mermaid
sequenceDiagram
    actor User
    participant UI as BorrowUtility UI
    participant Manager as LicenseManager
    participant Handler as FlmBorrowHandler
    participant FlexLM as lmborrow.exe
    participant Helper as BorrowHelper.exe
    
    User->>UI: Select Date & Click "Borrow"
    activate UI
    UI->>UI: Validate Date Input
    
    UI->>UI: Check if HarnessAnalyzer is running
    alt HarnessAnalyzer is running
        UI-->>User: Show warning to close HarnessAnalyzer
    else HarnessAnalyzer is not running
        UI->>Manager: GetAvailableFeaturesFromServer()
        activate Manager
        Manager->>Handler: SetAvailableFeaturesFromServerLicense()
        activate Handler
        Handler->>FlexLM: Execute lmstat.exe -i
        activate FlexLM
        FlexLM-->>Handler: Return available features
        deactivate FlexLM
        Handler-->>Manager: Return feature list
        deactivate Handler
        Manager-->>UI: Return ApplicationFeatureInfosResult
        deactivate Manager
        
        alt Features available
            UI->>Manager: BorrowFeaturesOrAvailable(features, date)
            activate Manager
            Manager->>Handler: BorrowLicense(date)
            activate Handler
            Handler->>FlexLM: Execute lmborrow.exe with vendor & date
            activate FlexLM
            FlexLM-->>Handler: Return success/failure
            deactivate FlexLM
            
            alt lmborrow success
                Handler->>Helper: Execute BorrowHelper.exe
                activate Helper
                Helper-->>Handler: Return result
                deactivate Helper
                
                Handler->>FlexLM: Execute lmborrow.exe -clear
                activate FlexLM
                FlexLM-->>Handler: Clear result
                deactivate FlexLM
                
                Handler-->>Manager: Return success/warnings
                deactivate Handler
                Manager-->>UI: Return BorrowResult
                deactivate Manager
                
                UI->>UI: Update status display
                UI-->>User: Show success/warning message
            else lmborrow failure
                Handler-->>Manager: Return error
                Manager-->>UI: Return failure
                UI-->>User: Show error message
            end
        else No features available
            UI-->>User: Show error message
        end
    end
    deactivate UI
```

### Sequence diagram explanation (Borrowing Operation)

1.	The process begins with user input (date selection and borrow button click) and follows a layered execution pattern through the UI, business logic, and external components

2.	Key validation steps include:
    - Input validation to ensure a valid date
    - Application state verification to prevent conflicts with running instances
    - Server connectivity checks and feature availability validation

3.	The actual borrowing involves a multi-step transaction:
    - First querying available features using lmstat.exe
    - Then initiating the borrow operation with lmborrow.exe
    - Completing the checkout using BorrowHelper.exe (a specialized component)
    - Finally clearing temporary state with another lmborrow.exe call

4.	Comprehensive error handling occurs at each stage, with different paths for:
    - Application conflicts (HarnessAnalyzer running)
    - Communication failures with license server
    - Missing features or authorization issues
    - FlexLM command execution failures

### How to Use BorrowUtility

#### For End Users (Post-Installation)

1. **Launch the Utility**:
   - The BorrowUtility is installed alongside E3.HarnessAnalyzer.
   - Launch `BorrowUtility.exe` from the installation directory.

2. **Borrow a License**:
   - Close E3.HarnessAnalyzer if it's running.
   - Set the desired expiration date.
   - Click the "Borrow License" button.
   - Wait for the process to complete.
   - Upon success, you can now use E3.HarnessAnalyzer offline until the expiration date.

3. **Return a License**:
   - When online access is restored, launch BorrowUtility.
   - Click the "Return License" button to release the borrowed license.
   - This makes the license available to other users.

4. **Advanced Operations** (via Debug menu):
   - View server licenses (see all available licenses on the server).
   - View logs (troubleshoot borrowing issues).
   - Edit license path (modify the license server configuration).
   - Reset FlexLM (resolve stubborn licensing issues).
   - View environment variables (check license-related environment settings).

#### For Developers

1. **Project Integration**:
   - Include the BorrowUtility project in your solution.
   - Ensure project references to dependencies are correctly set up.
   - Build configurations for multiple platforms (x86/x64) and frameworks (.NET Framework 4.7.2, .NET 6, .NET 8).

2. **Building the Project**:
   - Build the main BorrowUtility project.
   - The project has a reference to BorrowHelper to ensure correct build order.
   - Ensure all platform combinations are built (Debug/Release for x86/x64).

3. **Customizing License Features**:
   - Modify the `ApplicationFeature` enumeration in the Shared project to define licensable features.
   - Ensure the feature names match exactly with the FlexLM feature names (case-sensitive).

4. **Testing License Flows**:
   - Use the `/Debug` command-line switch to expose the debug menu.
   - Use the `/BuildServerTest` switch for CI/CD integration testing.

5. **Troubleshooting License Issues**:
   - Check the environment variables (ZUKEN_LICENSE_FILE).
   - Verify connectivity to the license server.
   - Ensure all required executables (lmborrow.exe, lmstat.exe, BorrowHelper.exe) are properly signed.
   - Check registry entries (under HKEY_CURRENT_USER\Software\FLEXlm License Manager\Borrow) for corrupted borrow information.

### Environment Requirements

1. **FlexLM Components**:
   - lmborrow.exe, lmstat.exe from FlexLM installed in the appropriate x86/x64 folder.
   - BorrowHelper.exe compiled for the target platform.
   - Flmac.dll (wrapper DLL) for the appropriate platform.

2. **License Server Configuration**:
   - Environment variable ZUKEN_LICENSE_FILE set to port@server (e.g., 27000@erl-build01).
   - For VMs, host file entries may be needed to resolve the server name.

3. **Security Requirements**:
   - All executable components must be digitally signed for security.
   - SigningChecker validates signatures before execution.

### Technical Notes

1. **Cross-Platform Compatibility**:
   - The utility is designed to work across multiple .NET framework versions.
   - Platform-specific code paths handle x86/x64 differences.

2. **Asynchronous Operations**:
   - License operations run asynchronously to prevent UI freezing.
   - A semaphore (`_lock`) ensures thread safety during operations.

3. **Error Handling**:
   - Comprehensive error reporting through the logging system.
   - User-friendly error messages with underlying technical details.

4. **Security Considerations**:
   - Executable signature validation prevents tampering.
   - Process execution is sandboxed with controlled parameters.

5. **Known Limitations**:
   - Feature names in FlexLM are case-sensitive.
   - The license checkout process requires a two-step approach (lmborrow.exe followed by BorrowHelper.exe).
   - Occasionally, registry entries may need manual cleanup for stubborn borrow entries.

This utility provides essential functionality for E3.HarnessAnalyzer users who need to work in disconnected environments, ensuring license compliance while enabling flexible workflows.