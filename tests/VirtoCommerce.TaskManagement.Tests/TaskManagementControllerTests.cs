using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.JsonConverters;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;
using VirtoCommerce.TaskManagement.Data.Authorization;
using VirtoCommerce.TaskManagement.Web.Controllers.Api;
using Xunit;
using TaskPermissions = VirtoCommerce.TaskManagement.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.TaskManagement.Tests;

[Trait("Category", "Unit")]
public class TaskManagementControllerTests
{
    private const string CurrentMemberId = "current-member";
    private const string OrganizationId = "organization";

    private readonly Mock<IMemberService> _memberService = new();
    private readonly Mock<IMemberSearchService> _memberSearchService = new();
    private readonly MvcNewtonsoftJsonOptions _jsonOptions = new();

    public TaskManagementControllerTests()
    {
        AbstractTypeFactory<PermissionScope>.RegisterType<TaskToMeScope>();
        AbstractTypeFactory<PermissionScope>.RegisterType<TaskToMyOrganizationScope>();
        PolymorphJsonConverter.RegisterTypeForDiscriminator(typeof(PermissionScope), nameof(PermissionScope.Type));
        _jsonOptions.SerializerSettings.Converters.Add(new PolymorphJsonConverter());

        AddMember(new Contact { Id = CurrentMemberId, Name = "Current", Organizations = [OrganizationId] });
        AddMember(new Contact { Id = "colleague", Name = "Colleague", Organizations = [OrganizationId] });
        AddMember(new Contact { Id = "stranger", Name = "Stranger", Organizations = ["other-organization"] });
        AddMember(new Organization { Id = OrganizationId, Name = "Organization" });
    }

    [Fact]
    public void Controller_RequiresAuthenticatedUser_ForEveryAction()
    {
        var controllerType = typeof(TaskManagementController);

        Assert.NotNull(controllerType.GetCustomAttribute<AuthorizeAttribute>());

        var anonymousActions = controllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(x => x.GetCustomAttribute<AllowAnonymousAttribute>() != null)
            .Select(x => x.Name)
            .ToList();

        Assert.Empty(anonymousActions);
    }

    [Fact]
    public async Task GetMemberById_WithoutPermission_ReturnsUnauthorized()
    {
        var controller = CreateController(CreateUser());

        var result = await controller.GetMemberById("stranger");

        Assert.IsType<UnauthorizedResult>(result.Result);
        _memberService.Verify(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetMemberById_WithGlobalPermission_LoadsMemberWithDefaultResponseGroup()
    {
        var controller = CreateController(CreateUser(new Claim(PlatformConstants.Security.Claims.PermissionClaimType, TaskPermissions.Create)));

        var result = await controller.GetMemberById("stranger");

        var member = Assert.IsType<TaskMember>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal("stranger", member.Id);
        _memberService.Verify(x => x.GetByIdAsync("stranger", nameof(MemberResponseGroup.Default), null), Times.Once);
        _memberService.Verify(x => x.GetByIdAsync(It.IsAny<string>(), It.Is<string>(r => r != nameof(MemberResponseGroup.Default)), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(CurrentMemberId, true)]
    [InlineData("colleague", false)]
    [InlineData(OrganizationId, false)]
    public async Task GetMemberById_WithTaskToMeScope_ReturnsOnlyOwnMember(string memberId, bool allowed)
    {
        var controller = CreateController(CreateUser(CreateScopedClaim(new TaskToMeScope())));

        var result = await controller.GetMemberById(memberId);

        AssertAllowed(result, allowed);
    }

    [Theory]
    [InlineData(CurrentMemberId, true)]
    [InlineData("colleague", true)]
    [InlineData(OrganizationId, true)]
    [InlineData("stranger", false)]
    public async Task GetMemberById_WithTaskToMyOrganizationScope_ReturnsOnlyOwnOrganizationMembers(string memberId, bool allowed)
    {
        var controller = CreateController(CreateUser(CreateScopedClaim(new TaskToMyOrganizationScope())));

        var result = await controller.GetMemberById(memberId);

        AssertAllowed(result, allowed);
    }

    [Fact]
    public async Task ContactIcon_WithoutPermission_ReturnsUnauthorized()
    {
        var controller = CreateController(CreateUser());

        var result = await controller.ContactIcon("stranger");

        Assert.IsType<UnauthorizedResult>(result);
        _memberService.Verify(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SearchAssignMembers_ReturnsOnlyTaskMemberFields_WithDefaultResponseGroup()
    {
        MembersSearchCriteria searched = null;
        _memberSearchService
            .Setup(x => x.SearchMembersAsync(It.IsAny<MembersSearchCriteria>()))
            .Callback<MembersSearchCriteria>(x => searched = x)
            .ReturnsAsync(new MemberSearchResult { TotalCount = 1, Results = [new Contact { Id = "stranger", Name = "Stranger", Emails = ["a@b.c"] }] });
        var controller = CreateController(CreateUser(new Claim(PlatformConstants.Security.Claims.PermissionClaimType, TaskPermissions.Create)));

        var result = await controller.SearchAssignMembers(new MembersSearchCriteria { ResponseGroup = nameof(MemberResponseGroup.Full) });

        var searchResult = Assert.IsType<TaskMemberSearchResult>(Assert.IsType<OkObjectResult>(result.Result).Value);
        var member = Assert.Single(searchResult.Results);
        Assert.Equal(("stranger", "Stranger", nameof(Contact)), (member.Id, member.Name, member.MemberType));
        Assert.Equal(nameof(MemberResponseGroup.Default), searched.ResponseGroup);
    }

    [Fact]
    public async Task SearchAssignMembers_WithTaskToMeScope_IgnoresOtherObjectIds()
    {
        var controller = CreateController(CreateUser(CreateScopedClaim(new TaskToMeScope())));

        var result = await controller.SearchAssignMembers(new MembersSearchCriteria { ObjectIds = ["stranger"] });

        var searchResult = Assert.IsType<TaskMemberSearchResult>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Empty(searchResult.Results);
        _memberSearchService.Verify(x => x.SearchMembersAsync(It.IsAny<MembersSearchCriteria>()), Times.Never);
    }

    [Fact]
    public async Task SearchAssignMembers_WithTaskToMyOrganizationScope_SearchesOwnOrganizationOnly()
    {
        MembersSearchCriteria searched = null;
        _memberSearchService
            .Setup(x => x.SearchMembersAsync(It.IsAny<MembersSearchCriteria>()))
            .Callback<MembersSearchCriteria>(x => searched = x)
            .ReturnsAsync(new MemberSearchResult { Results = [] });
        var controller = CreateController(CreateUser(CreateScopedClaim(new TaskToMyOrganizationScope())));

        await controller.SearchAssignMembers(new MembersSearchCriteria { ObjectIds = ["stranger"] });

        Assert.Equal(OrganizationId, searched.MemberId);
    }

    private static void AssertAllowed(ActionResult<TaskMember> result, bool allowed)
    {
        if (allowed)
        {
            Assert.IsType<OkObjectResult>(result.Result);
        }
        else
        {
            Assert.IsType<UnauthorizedResult>(result.Result);
        }
    }

    private void AddMember(Member member)
    {
        _memberService
            .Setup(x => x.GetByIdAsync(member.Id, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(member);
    }

    private Claim CreateScopedClaim(PermissionScope scope)
    {
        var permission = new Permission { Name = TaskPermissions.Create, AssignedScopes = [scope] };
        return permission.ToClaim(_jsonOptions.SerializerSettings);
    }

    private static ClaimsPrincipal CreateUser(params Claim[] claims)
    {
        var allClaims = new List<Claim> { new("memberId", CurrentMemberId) };
        allClaims.AddRange(claims);
        return new ClaimsPrincipal(new ClaimsIdentity(allClaims, "Bearer"));
    }

    private TaskManagementController CreateController(ClaimsPrincipal user)
    {
        var controller = new TaskManagementController(
            Mock.Of<IWorkTaskService>(),
            Mock.Of<IWorkTaskSearchService>(),
            _memberService.Object,
            _memberSearchService.Object,
            Mock.Of<ISettingsManager>(),
            Mock.Of<IAuthorizationService>(),
            Options.Create(_jsonOptions));

        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

        return controller;
    }
}
