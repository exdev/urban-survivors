// ======================================================================================
// File         : drawlineTest.js
// Author       : Wu Jie 
// Last Change  : 08/25/2010 | 22:11:56 PM | Wednesday,August
// Description  : 
// ======================================================================================

static var lineMaterial : Material;

static function CreateLineMaterial()
{
    if( !lineMaterial ) {
        lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
            "SubShader { Pass { " +
            "    Blend SrcAlpha OneMinusSrcAlpha " +
            "    ZWrite Off Cull Off Fog { Mode Off } " +
            "    BindChannels {" +
            "      Bind \"vertex\", vertex Bind \"color\", color }" +
            "} } }" );
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
    }
}

function OnPostRender()
{
    CreateLineMaterial();
    // set the current material
    lineMaterial.SetPass( 0 );
    GL.Begin( GL.LINES );
    GL.Color( Color(1,1,1,0.5) );
    GL.Vertex3( 0, 0, 0 );
    GL.Vertex3( 1, 0, 0 );
    GL.Vertex3( 0, 1, 0 );
    GL.Vertex3( 1, 1, 0 );
    GL.Color( Color(0,0,0,0.5) );
    GL.Vertex3( 0, 0, 0 );
    GL.Vertex3( 0, 1, 0 );
    GL.Vertex3( 1, 0, 0 );
    GL.Vertex3( 1, 1, 0 );
    GL.End();
} 
