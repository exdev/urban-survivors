-- local file = io.open ("data.txt")
local file = io.stdin

if file then
    -- io.write( "echo ========== process changes ==========\n" )

    -- process input
    for line in file:lines() do
        -- local _,merge_type,path = unpack(line:split(" "))
        local merge_type,path = line:match("#%s+(.+):%s+(.+)")
        if merge_type and path then
            -- DEBUG: io.write( merge_type .. ": " .. path .. "\n" )

            -- changes
            -- if merge_type == "new file" then
            --     io.write( "git add " .. path .. "\n" )
            -- elseif merge_type == "modified" then
            --     io.write( "git add " .. path .. "\n" )
            -- elseif merge_type == "renamed" then
            --     -- NOTE: - is a pattern character, so you have to use %-
            --     local src,dest = path:match("(.+)%s%->%s(.+)")
            --     io.write( "git mv " .. src .. " " .. dest .. "\n" )
            -- elseif merge_type == "deleted" then
            --     io.write( "git rm " .. path .. "\n" )

            -- unmerged
            if merge_type == "added by them" then
                io.write( "git checkout --theirs " .. path .. "\n" ) 
                io.write( "git add " .. path .. "\n" ) 
            elseif merge_type == "added by us" then 
                io.write( "git checkout --ours " .. path .. "\n" ) 
                io.write( "git add " .. path .. "\n" ) 
            elseif merge_type == "both deleted" then 
                io.write( "git rm " .. path .. "\n" ) 
            elseif merge_type == "both modified" then 
                io.write( "git checkout --theirs " .. path .. "\n" ) 
                io.write( "git add " .. path .. "\n" ) 
            end
        end
    end
    io.write( "git add .\n" ) 
else
    assert ( false, "failed to open file data.txt" )
end
