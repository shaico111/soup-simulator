#!/bin/bash

# Create directories if they don't exist
mkdir -p wwwroot/images/soup
mkdir -p wwwroot/correct_answers/soup

# Function to download an image
download_image() {
    local filename=$1
    local color=$2
    local text=$3
    curl -o "wwwroot/images/soup/$filename" "https://placehold.co/400x300/$color/FFFFFF/png?text=$text"
}

# Question 1 - Red soup theme
download_image "soup1_q.jpg" "FF0000" "Red%20Soup"
download_image "soup1_correct.jpg" "FF0000" "Correct%20Answer"
download_image "soup1_a.jpg" "FF3333" "Option%20A"
download_image "soup1_b.jpg" "FF6666" "Option%20B"
download_image "soup1_c.jpg" "FF9999" "Option%20C"
download_image "soup1_d.jpg" "FFCCCC" "Option%20D"
download_image "soup1_e.jpg" "FFE6E6" "Option%20E"
download_image "soup1_f.jpg" "FFD6D6" "Option%20F"
download_image "soup1_g.jpg" "FFC6C6" "Option%20G"
download_image "soup1_h.jpg" "FFB6B6" "Option%20H"

# Question 2 - Green soup theme
download_image "soup2_q.jpg" "00FF00" "Green%20Soup"
download_image "soup2_correct.jpg" "00FF00" "Correct%20Answer"
download_image "soup2_a.jpg" "33FF33" "Option%20A"
download_image "soup2_b.jpg" "66FF66" "Option%20B"
download_image "soup2_c.jpg" "99FF99" "Option%20C"
download_image "soup2_d.jpg" "CCFFCC" "Option%20D"
download_image "soup2_e.jpg" "E6FFE6" "Option%20E"
download_image "soup2_f.jpg" "D6FFD6" "Option%20F"
download_image "soup2_g.jpg" "C6FFC6" "Option%20G"
download_image "soup2_h.jpg" "B6FFB6" "Option%20H"

# Question 3 - Blue soup theme
download_image "soup3_q.jpg" "0000FF" "Blue%20Soup"
download_image "soup3_correct.jpg" "0000FF" "Correct%20Answer"
download_image "soup3_a.jpg" "3333FF" "Option%20A"
download_image "soup3_b.jpg" "6666FF" "Option%20B"
download_image "soup3_c.jpg" "9999FF" "Option%20C"
download_image "soup3_d.jpg" "CCCCFF" "Option%20D"
download_image "soup3_e.jpg" "E6E6FF" "Option%20E"
download_image "soup3_f.jpg" "D6D6FF" "Option%20F"
download_image "soup3_g.jpg" "C6C6FF" "Option%20G"
download_image "soup3_h.jpg" "B6B6FF" "Option%20H"

# Question 4 - Yellow soup theme
download_image "soup4_q.jpg" "FFFF00" "Yellow%20Soup"
download_image "soup4_correct.jpg" "FFFF00" "Correct%20Answer"
download_image "soup4_a.jpg" "FFFF33" "Option%20A"
download_image "soup4_b.jpg" "FFFF66" "Option%20B"
download_image "soup4_c.jpg" "FFFF99" "Option%20C"
download_image "soup4_d.jpg" "FFFFCC" "Option%20D"
download_image "soup4_e.jpg" "FFFFE6" "Option%20E"
download_image "soup4_f.jpg" "FFFFD6" "Option%20F"
download_image "soup4_g.jpg" "FFFFC6" "Option%20G"
download_image "soup4_h.jpg" "FFFFB6" "Option%20H"

# Question 5 - Purple soup theme
download_image "soup5_q.jpg" "800080" "Purple%20Soup"
download_image "soup5_correct.jpg" "800080" "Correct%20Answer"
download_image "soup5_a.jpg" "993399" "Option%20A"
download_image "soup5_b.jpg" "B366B3" "Option%20B"
download_image "soup5_c.jpg" "CC99CC" "Option%20C"
download_image "soup5_d.jpg" "E6CCE6" "Option%20D"
download_image "soup5_e.jpg" "F2E6F2" "Option%20E"
download_image "soup5_f.jpg" "EBD6EB" "Option%20F"
download_image "soup5_g.jpg" "E5CCE5" "Option%20G"
download_image "soup5_h.jpg" "DFC3DF" "Option%20H"

echo "Downloaded all soup images!" 