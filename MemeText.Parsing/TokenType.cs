using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeText.Parsing
{
    public enum TokenType
    {
        Text,
        EndAuto,            // **
        StartEmphasis,      // *e
        EndEmphasis,        // e*

        StartStrong,        // *s
        EndStrong,          // s*

        StartListItem,      // *-
        ListItemBox,        // [ ]
        ListItemComplete,   // [X]
        ListItemPct,        // [0-100]
        ListItemCancelled,  // [~]
        EndListItem,        // -*

        StartComment,       // *!
        EndComment,         // !*

        StartItemLink,      // *~
        EndItemLink,        // ~*

        StartUserLink,      // *@
        EndUserLink,        // @*

        StartAnchor,        // *a
        EndAnchor,          // a*

        StartImage,         // *i
        EndImage,           // i*

        URI,                // [http://domain.com/] in *a or *i, can be #footnote, or *{key}* (?)

        StartFootnote,      // *# ... [footnote] value
        EndFootnote,        // #*

        StartCode,          // *+
        EndCode,            // +*

        StartEmoji,         // *:
        EndEmoji,           // :*

        LineBreak,          // CR/LF
        Indent,             // Indent by one level
        Dedent,             // Dedent by one level

        StartUnknown,       // *<something-else>
        EndUnknown,         // <somethine-else>*
    }
}
