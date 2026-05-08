Public Structure FaceTexturValues
    Public useDifferentInnerArea As Boolean
    Public innerCloudStrength As Single
    Public innerGrainStrength As Single
    Public outerCloudStrength As Single
    Public outerGrainStrength As Single
    Public seed As Integer?
    Public steinStatus As SteinStatus
    Public Shadows ReadOnly Property ToString As String
        Get
            Return $"OuterCloud={outerCloudStrength}, OuterGrain={outerGrainStrength}, InnerCloud={innerCloudStrength}, InnerGrain={innerGrainStrength}, UseInner={useDifferentInnerArea}, seed={If(seed, "nothing")}"
        End Get
    End Property

End Structure
