// ======================================================================================
// File         : Btree.js
// Author       : Wu Jie 
// Last Change  : 09/14/2010 | 23:15:13 PM | Tuesday,September
// Description  : 
// ======================================================================================

#pragma strict

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// class BehaveTree 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class BehaveTree {
    var root : BTNode;
    function BehaveTree ( name : String ) {
        Debug.Log (name);
    }
    function init ( _root : BTNode ) {
        root = _root;
    }
    function tick () : boolean {
        return root.exec();
    }
}

///////////////////////////////////////////////////////////////////////////////
// class BTNode
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class BTNode {
    function BTNode () { 
    }
    virtual function exec () : boolean {
        Debug.Log ("warnnig: please implement exec in your node!");
        return false;
    }
}

///////////////////////////////////////////////////////////////////////////////
// class BTPrim 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class BTPrim extends BTNode {
    virtual function exec () : boolean { 
        Debug.Log ("BTPrim::exec invoked!");
        return true;
    }
}

///////////////////////////////////////////////////////////////////////////////
// class BTCond
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class BTCond extends BTPrim {
    virtual function exec () : boolean {
        Debug.Log ("BTCond::exec invoked!");
        return true;
    }
}

///////////////////////////////////////////////////////////////////////////////
// class BTAct 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class BTAct extends BTPrim {
    virtual function exec () : boolean {
        Debug.Log ("BTAct::exec invoked!");
        return true;
    }
}

///////////////////////////////////////////////////////////////////////////////
// class BTComp 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class BTComp extends BTNode {
    var sub_nodes : BTNode[];

    function BTComp ( _nodes : Array ) {
        super();
        this.sub_nodes = _nodes;
    }

    virtual function exec () : boolean {
        Debug.Log ("BTComp::exec invoked!");
        return true;
    }
}

///////////////////////////////////////////////////////////////////////////////
// class BTSeq 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class BTSeq extends BTComp {
    function BTSeq ( _nodes : Array ) {
        super(_nodes);
    }
    virtual function exec () : boolean {
        for ( i = 0; i < this.sub_nodes.length; ++i ) {
            if ( sub_nodes[i].exec() == false ) {
                return false;
            }
        }
        return true;
    }
}

///////////////////////////////////////////////////////////////////////////////
// class BTSel 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class BTSel extends BTComp {
    function BTSel ( _nodes : Array ) {
        super (_nodes);
    }
    virtual function exec () : boolean {
        for ( i = 0; i < this.sub_nodes.length; ++i ) {
            if ( sub_nodes[i].exec() ) {
                return true;
            }
        }
        return false;
    }
}

///////////////////////////////////////////////////////////////////////////////
// class BTPrl 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class BTPrl extends BTComp {
    function BTPrl ( _nodes : Array ) {
        super (_nodes);
    }
    virtual function exec () : boolean {
        // TODO { 
        // for ( i = 0; i < this.sub_nodes.length; ++i ) {
        //     // MonoBehaviour.StartCoroutine ( sub_nodes[i].exec() );
        //     sub_nodes[i].exec();
        // }
        // // TODO: get coroutine result
        // } TODO end 
        return false;
    }
}

///////////////////////////////////////////////////////////////////////////////
// class BTDec 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class BTDec extends BTComp {
    function BTDec ( _nodes : Array ) {
        super (_nodes);
    }
    virtual function exec () : boolean {
        // TODO: assert sub_nodes only have one item.
        return this.sub_nodes[0].exec();
    }
}
