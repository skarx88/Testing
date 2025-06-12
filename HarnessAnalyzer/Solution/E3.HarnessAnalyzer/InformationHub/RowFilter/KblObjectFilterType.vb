<Flags>
Public Enum KblObjectFilterType
    Unfiltered = 0
    Filtered = 1
    ForceGrayOut = 2
    FilteredAndGrayOut = Filtered Or KblObjectFilterType.ForceGrayOut
End Enum