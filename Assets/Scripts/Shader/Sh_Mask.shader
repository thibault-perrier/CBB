Shader "Custom/Sh_Mask"
{
    SubShader{
        Tags{ "Queue" = "Transparent+1"}

        Pass{
            Blend Zero One 
        }
    }
}
