#!/bin/bash

# Create noodles directory if it doesn't exist
mkdir -p wwwroot/images/noodles

# Function to convert PNG to JPG
convert_to_jpg() {
    local input=$1
    local output=$2
    # Using ImageMagick if available, otherwise just copy
    if command -v convert &> /dev/null; then
        convert "$input" "$output"
    else
        cp "$input" "$output"
    fi
}

# Group 1
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-24-25.png" "wwwroot/images/noodles/noodle1_q.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-24-28.png" "wwwroot/images/noodles/noodle1_correct.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-25-37.png" "wwwroot/images/noodles/noodle1_a.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-25-48.png" "wwwroot/images/noodles/noodle1_b.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-26-00.png" "wwwroot/images/noodles/noodle1_c.jpg"

# Group 2
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-26-03.png" "wwwroot/images/noodles/noodle2_q.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-26-06.png" "wwwroot/images/noodles/noodle2_correct.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-26-08.png" "wwwroot/images/noodles/noodle2_a.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-26-23.png" "wwwroot/images/noodles/noodle2_b.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-26-38.png" "wwwroot/images/noodles/noodle2_c.jpg"

# Group 3
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-26-41.png" "wwwroot/images/noodles/noodle3_q.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-26-44.png" "wwwroot/images/noodles/noodle3_correct.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-26-47.png" "wwwroot/images/noodles/noodle3_a.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-27-05.png" "wwwroot/images/noodles/noodle3_b.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-27-23.png" "wwwroot/images/noodles/noodle3_c.jpg"

# Group 4
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-27-26.png" "wwwroot/images/noodles/noodle4_q.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-27-29.png" "wwwroot/images/noodles/noodle4_correct.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-27-31.png" "wwwroot/images/noodles/noodle4_a.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-27-51.png" "wwwroot/images/noodles/noodle4_b.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-28-03.png" "wwwroot/images/noodles/noodle4_c.jpg"

# Group 5
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-28-06.png" "wwwroot/images/noodles/noodle5_q.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-28-08.png" "wwwroot/images/noodles/noodle5_correct.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-28-10.png" "wwwroot/images/noodles/noodle5_a.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-28-18.png" "wwwroot/images/noodles/noodle5_b.jpg"
convert_to_jpg "wwwroot/images/Screenshot at Apr 20 16-28-19.png" "wwwroot/images/noodles/noodle5_c.jpg"

echo "Organized noodle images successfully!" 