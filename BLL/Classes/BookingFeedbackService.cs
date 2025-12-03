using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using DAL.Models;
using DAL.Repositories;

namespace BLL.Classes
{
    public class BookingFeedbackService : IBookingFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingFeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseWithPagination<List<BookingFeedbackResponseDto>>> GetAllAsync(PagedRequestDto request)
        {
            var (items, total) = await _unitOfWork.BookingFeedbackRepo.GetPagedAsync(request.Page, request.Limit);

            var responseDtos = items.Select(f => new BookingFeedbackResponseDto
            {
                FeedbackId = f.FeedbackId,
                BookingId = f.BookingId,
                UserId = f.UserId,
                UserName = f.User?.FullName ?? string.Empty,
                Rating = f.Rating,
                Comments = f.Comments,
                ReportIssue = f.ReportIssue,
                IssueDescription = f.IssueDescription,
                CreatedAt = f.CreatedAt,
                ResolvedAt = f.ResolvedAt
            }).ToList();

            return ApiResponseWithPagination<List<BookingFeedbackResponseDto>>.Ok(
                responseDtos,
                request.Page,
                request.Limit,
                total
            );
        }

        public async Task<ApiResponse<BookingFeedbackResponseDto>> GetByIdAsync(string id)
        {
            var feedback = await _unitOfWork.BookingFeedbackRepo.GetByIdAsync(id);
            if (feedback == null)
            {
                return ApiResponse<BookingFeedbackResponseDto>.Fail(404, "Feedback not found");
            }

            var responseDto = new BookingFeedbackResponseDto
            {
                FeedbackId = feedback.FeedbackId,
                BookingId = feedback.BookingId,
                UserId = feedback.UserId,
                UserName = feedback.User?.FullName ?? string.Empty,
                Rating = feedback.Rating,
                Comments = feedback.Comments,
                ReportIssue = feedback.ReportIssue,
                IssueDescription = feedback.IssueDescription,
                CreatedAt = feedback.CreatedAt,
                ResolvedAt = feedback.ResolvedAt
            };

            return ApiResponse<BookingFeedbackResponseDto>.Ok(responseDto);
        }

        public async Task<ApiResponse<BookingFeedbackResponseDto>> CreateAsync(CreateBookingFeedbackDto dto)
        {
            var feedbackId = await GenerateFeedbackIdAsync();

            var feedback = new BookingFeedback
            {
                FeedbackId = feedbackId,
                BookingId = dto.BookingId,
                UserId = dto.UserId,
                Rating = dto.Rating,
                Comments = dto.Comments,
                ReportIssue = dto.ReportIssue,
                IssueDescription = dto.IssueDescription,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BookingFeedbackRepo.CreateAsync(feedback);

            var responseDto = new BookingFeedbackResponseDto
            {
                FeedbackId = feedback.FeedbackId,
                BookingId = feedback.BookingId,
                UserId = feedback.UserId,
                Rating = feedback.Rating,
                Comments = feedback.Comments,
                ReportIssue = feedback.ReportIssue,
                IssueDescription = feedback.IssueDescription,
                CreatedAt = feedback.CreatedAt
            };

            return ApiResponse<BookingFeedbackResponseDto>.Ok(responseDto);
        }

        private async Task<string> GenerateFeedbackIdAsync()
        {
            var feedbacks = await _unitOfWork.BookingFeedbackRepo.GetAllAsync();
            var maxId = 0;

            foreach (var feedback in feedbacks)
            {
                if (feedback.FeedbackId.StartsWith("BF") && feedback.FeedbackId.Length == 6)
                {
                    if (int.TryParse(feedback.FeedbackId.Substring(2), out var id))
                    {
                        if (id > maxId)
                            maxId = id;
                    }
                }
            }

            return $"BF{(maxId + 1):D4}";
        }
    }
}

