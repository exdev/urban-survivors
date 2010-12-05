-- local file = io.open ("data.txt")
local file = io.stdin

-- unmerged changes
local added_by_them = {}
local added_by_us = {}
local both_modified = {}
local both_deleted = {}

if file then
    io.write( "echo ========== process changes ==========\n" )

    -- process input
    for line in file:lines() do
        -- local _,merge_type,path = unpack(line:split(" "))
        local merge_type,path = line:match("#%s+(.+):%s+(.+)")
        if merge_type and path then
            -- DEBUG: io.write( merge_type .. ": " .. path .. "\n" )

            -- changes
            if merge_type == "new file" then
                io.write( "git add " .. path .. "\n" )
            elseif merge_type == "modified" then
                io.write( "git add " .. path .. "\n" )
            elseif merge_type == "renamed" then
                -- NOTE: - is a pattern character, so you have to use %-
                local src,dest = path:match("(.+)%s%->%s(.+)")
                io.write( "git mv " .. src .. " " .. dest .. "\n" )
            elseif merge_type == "deleted" then
                io.write( "git rm " .. path .. "\n" )

            -- unmerged
            elseif merge_type == "added by them" then
                added_by_them[#added_by_them+1] = path
            elseif merge_type == "added by us" then 
                added_by_us[#added_by_us+1] = path
            elseif merge_type == "both deleted" then 
                both_deleted[#both_deleted+1] = path
            elseif merge_type == "both modified" then 
                both_modified[#both_modified+1] = path
            end
        end
    end

    -- process output
    io.write( "echo ========== process merges ==========\n" )
    io.write( "echo ---------- added by them ----------\n" )
    for i=1,#added_by_them do 
        local path = added_by_them[i]
        io.write( "git checkout --theirs " .. path .. "\n" ) 
        io.write( "git add " .. path .. "\n" ) 
    end

    io.write( "echo ---------- added by us ----------\n" )
    for i=1,#added_by_us do 
        local path = added_by_us[i]
        io.write( "git checkout --ours " .. path .. "\n" ) 
        io.write( "git add " .. path .. "\n" ) 
    end

    io.write( "echo ---------- both modified ----------\n" )
    for i=1,#both_modified do 
        local path = both_modified[i]
        io.write( "git checkout --theirs " .. path .. "\n" ) 
        io.write( "git add " .. path .. "\n" ) 
    end

    io.write( "echo ---------- both deleted ----------\n" ) -- NOTE: always manipluate delete at last
    for i=1,#both_deleted do 
        local path = both_deleted[i]
        io.write( "git rm " .. path .. "\n" ) 
    end
else
    assert ( false, "failed to open file data.txt" )
end
