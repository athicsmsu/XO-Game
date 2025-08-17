# XO-Game (Unity 2D Mobile)

เกม XO (Tic-Tac-Toe) เวอร์ชันมือถือ พัฒนาด้วย Unity 2D  
รองรับ Android เล่นได้ทั้งแบบผู้เล่น 2 คน (Local Multiplayer) และโหมดเล่นกับบอท (AI)

---

## คุณสมบัติ
- 🎮 ตาราง XO ขนาด 3x3 (ปรับได้สูงสุด 10x10)  
- 👤 เล่นกับเพื่อน (Local Multiplayer)  
- 🤖 เล่นกับบอท (AI)  
- 📱 รองรับ Android  
- 💾 บันทึกผลแพ้-ชนะ และดูย้อนหลังได้  

---

## วิธีเล่น

- เลือกโหมด Player vs Player หรือ Player vs Bot
- กดเลือก cell เพื่อวาง X หรือ O
- เมื่อมีผู้ชนะหรือเสมอ → แสดงผลบนหน้าจอ
- สามารถ Replay หรือดูประวัติผลแพ้-ชนะได้(เก็บเฉพาะกรณีที่จบเกมจากการชนะหรือเสมอ)

## ภาพตัวอย่าง 
![Screenshot](Assets/Screenshots/gameplay.png)

![Screenshot](Assets/Screenshots/gameplay2.png)

![Screenshot](Assets/Screenshots/gameplay3.png)

---

## การติดตั้ง

### สำหรับ Android (APK)
1. ดาวน์โหลดไฟล์ [XO Game APK](XO%20Game%20APK.zip)  
2. ติดตั้งลงบนมือถือ Android ของคุณ  
   ```bash
   adb install "XO Game.apk"
   
## สำหรับ Unity

### การติดตั้งและรันโปรเจกต์

1. Clone หรือดาวน์โหลดโปรเจกต์จาก GitHub
   ```bash
   git clone https://github.com/athicsmsu/XO-Game.git
2. เปิดโปรเจกต์ด้วย Unity 2021+
3. เปิด Scene หลัก Assets/Scenes/Home.unity
4. ตั้งค่า Build Settings → Platform: Android
5. กด Play เพื่อทดสอบใน Editor หรือ Build & Run สำหรับ Android

## Algorithm หลัก
### 1. ตรวจสอบผู้ชนะ (Win Condition)
- ตรวจสอบทุก **Row, Column, Diagonal**  
- ถ้า player ใดมีเครื่องหมายเรียงกันครบ `boardSize` → ผู้ชนะ  

### 2. Bot AI (ง่าย)
- เลือกตำแหน่งว่างแบบ **สุ่ม**  
- *(สามารถปรับเป็น Minimax Algorithm สำหรับ AI เก่งขึ้นในอนาคต)*  

### 3. การจัดการ Grid
- ใช้ **Button[,] 2D array** แทนแต่ละ cell  
- เมื่อผู้เล่นกด cell → เรียก `OnClickCell(row, col)`  
- Update UI ด้วย Sprite ของ X หรือ O  

### 4. Database (บันทึกผลแพ้-ชนะ)
- ใช้ **SQLite** ผ่าน **DBManager** ในการบันทึกผลเกมทุกครั้งเมื่อจบเกม  
- ข้อมูลที่บันทึก:
  - ชื่อผู้เล่น 1 และ 2  
  - ผลลัพธ์ของเกม (ชนะ/แพ้/เสมอ)  
  - วันเวลาเกม  
- สามารถดึงข้อมูลมาแสดง **ประวัติการเล่นย้อนหลัง** ได้  
- SQLite ทำให้สามารถเก็บข้อมูลภายในเครื่อง **ไม่ต้องเชื่อมต่ออินเทอร์เน็ต** และเรียกดูได้ทันที
